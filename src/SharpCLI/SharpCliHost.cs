namespace SharpCLI;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;
using Models;
using ParameterInfo = Models.ParameterInfo;

/// <summary>
/// Main host class for SharpCli framework that manages CLI commands, arguments, and options.
/// Provides an attribute-based approach for building command-line applications in C#.
/// </summary>
/// <param name="name">The name of the CLI application</param>
/// <param name="description">Optional description of the CLI application</param>
public class SharpCliHost(string name, string description = "")
{
    /// <summary>
    /// Dictionary storing all registered commands by their names
    /// </summary>
    private readonly Dictionary<string, CommandInfo> _commands = new();

    /// <summary>
    /// Dictionary mapping command aliases to their actual command names
    /// </summary>
    private readonly Dictionary<string, string> _aliases = new();

    /// <summary>
    /// Cache of compiled factory functions that generate default values for types.
    /// This provides high-performance type-safe default value creation while avoiding
    /// trimming issues with Activator.CreateInstance.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, Func<object>> DefaultValueFactories =
        new ConcurrentDictionary<Type, Func<object>>();

    /// <summary>
    /// Registers all methods marked with [Command] attribute from an instance object.
    /// This allows for instance-based commands where the object can maintain state.
    /// </summary>
    /// <param name="commandsObject">Object containing command methods</param>
    /// <returns>Current SharpCliHost instance for method chaining</returns>
    public SharpCliHost RegisterCommands(object commandsObject)
    {
        var type = commandsObject.GetType();
        // Get both instance and static methods from the object
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

        foreach (var method in methods)
        {
            var commandAttr = method.GetCustomAttribute<CommandAttribute>();
            if (commandAttr != null)
            {
                RegisterCommand(commandsObject, method, commandAttr);
            }
        }

        return this;
    }

    /// <summary>
    /// Registers all static methods marked with [Command] attribute from a type.
    /// This is the most common registration method for stateless commands.
    /// </summary>
    /// <typeparam name="T">Type containing static command methods</typeparam>
    /// <returns>Current SharpCliHost instance for method chaining</returns>
    public SharpCliHost RegisterCommands<T>() where T : class
    {
        var type = typeof(T);
        // Only get static methods since no instance is provided
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

        foreach (var method in methods)
        {
            var commandAttr = method.GetCustomAttribute<CommandAttribute>();
            if (commandAttr != null)
            {
                RegisterCommand(null, method, commandAttr);
            }
        }

        return this;
    }

    /// <summary>
    /// Internal method that processes a single command method and registers it in the system.
    /// Analyzes method parameters to extract arguments and options information.
    /// </summary>
    /// <param name="instance">Instance object (null for static methods)</param>
    /// <param name="method">Method info of the command</param>
    /// <param name="commandAttr">Command attribute containing metadata</param>
    private void RegisterCommand(object? instance, MethodInfo method, CommandAttribute commandAttr)
    {
        var parameters = method.GetParameters();
        var parameterInfos = new List<ParameterInfo>();

        // Process each method parameter to determine if it's an argument or option
        foreach (var param in parameters)
        {
            var argAttr = param.GetCustomAttribute<ArgumentAttribute>();
            var optAttr = param.GetCustomAttribute<OptionAttribute>();

            if (argAttr != null)
            {
                // Parameter is marked as an argument (positional parameter)
                parameterInfos.Add(new ParameterInfo
                {
                    Name = argAttr.Name,
                    Type = param.ParameterType,
                    IsArgument = true,
                    Required = argAttr.Required,
                    Description = argAttr.Description,
                    Position = parameterInfos.Count(p => p.IsArgument) // Auto-assign position
                });
            }
            else if (optAttr != null)
            {
                // Parameter is marked as an option (named parameter with flags)
                parameterInfos.Add(new ParameterInfo
                {
                    Name = param.Name!,
                    Type = param.ParameterType,
                    IsOption = true,
                    Required = !param.HasDefaultValue,
                    // Use attribute default value, then parameter default, then type default
                    DefaultValue = optAttr.DefaultValue ??
                                   (param.HasDefaultValue ? param.DefaultValue : GetDefaultValue(param.ParameterType)),
                    Description = optAttr.Description,
                    ShortName = optAttr.ShortName,
                    LongName = optAttr.LongName
                });
            }
            else
            {
                // Parameter without attribute - treat as positional argument
                parameterInfos.Add(new ParameterInfo
                {
                    Name = param.Name!,
                    Type = param.ParameterType,
                    IsArgument = true,
                    Required = !param.HasDefaultValue,
                    DefaultValue = param.HasDefaultValue ? param.DefaultValue : null,
                    Position = parameterInfos.Count(p => p.IsArgument)
                });
            }
        }

        // Determine if the method is async by checking return type
        var isAsync = method.ReturnType == typeof(Task<int>) || method.ReturnType == typeof(Task);

        // Create command info object to store all command metadata
        var commandInfo = new CommandInfo
        {
            Name = commandAttr.Name,
            Description = commandAttr.Description,
            Aliases = commandAttr.Aliases,
            Method = method,
            Instance = instance,
            Parameters = parameterInfos.ToArray(),
            IsAsync = isAsync
        };

        // Register the command by name
        _commands[commandAttr.Name] = commandInfo;

        // Register all aliases for this command
        foreach (var alias in commandAttr.Aliases)
        {
            _aliases[alias] = commandAttr.Name;
        }
    }

    /// <summary>
    /// Main entry point for the CLI application. Parses command-line arguments,
    /// finds the appropriate command, and executes it with parsed parameters.
    /// </summary>
    /// <param name="args">Command-line arguments from Main method</param>
    /// <returns>Exit code (0 for success, 1 for error)</returns>
    public async Task<int> RunAsync(string[] args)
    {
        // Show help if no arguments provided
        if (args.Length == 0)
        {
            ShowHelp();
            return 0;
        }

        var commandName = args[0];

        // Handle global help commands
        if (commandName is "--help" or "-h" or "help")
        {
            ShowHelp();
            return 0;
        }

        // Resolve command aliases to actual command names
        if (_aliases.ContainsKey(commandName))
        {
            commandName = _aliases[commandName];
        }

        // Check if command exists
        if (!_commands.ContainsKey(commandName))
        {
            Console.WriteLine($"Unknown command: {commandName}");
            Console.WriteLine($"Use '{name} --help' for usage information.");
            return 1;
        }

        var command = _commands[commandName];
        var commandArgs = args.Skip(1).ToArray(); // Remove command name from arguments

        try
        {
            // Handle command-specific help
            if (commandArgs.Contains("--help") || commandArgs.Contains("-h"))
            {
                ShowCommandHelp(commandName, command);
                return 0;
            }

            // Parse command arguments and options into method parameters
            var parameters = ParseArguments(command, commandArgs);

            // Execute the command method
            if (command.IsAsync)
            {
                // Handle async methods (Task<int> or Task)
                var result = command.Method.Invoke(command.Instance, parameters);
                switch (result)
                {
                    case Task<int> taskInt:
                        return await taskInt; // Return the int result from Task<int>
                    case Task task:
                        await task;
                        return 0; // Task without return value, assume success
                    default:
                        return 0; // Fallback for unexpected return types
                }
            }
            else
            {
                // Handle synchronous methods
                var result = command.Method.Invoke(command.Instance, parameters);
                if (result is int intResult)
                    return intResult; // Return the int result

                return 0; // Void methods or unexpected return types, assume success
            }
        }
        catch (Exception ex)
        {
            // Handle any errors during command execution
            Console.WriteLine($"Error executing command: {ex.Message}");
            return 1;
        }
    }

    /// <summary>
    /// Parses command-line arguments into method parameters.
    /// Handles both positional arguments and named options with their values.
    /// </summary>
    /// <param name="command">Command info containing parameter definitions</param>
    /// <param name="args">Command-line arguments (excluding the command name)</param>
    /// <returns>Array of parsed parameter values in method signature order</returns>
    private object[] ParseArguments(CommandInfo command, string[] args)
    {
        // Separate arguments (positional) from options (named)
        var arguments = command.Parameters.Where(p => p.IsArgument).OrderBy(p => p.Position).ToArray();
        var options = command.Parameters.Where(p => p.IsOption).ToArray();
        var result = new object[command.Parameters.Length];

        // Initialize all parameters with their default values
        for (var i = 0; i < command.Parameters.Length; i++)
        {
            result[i] = command.Parameters[i].DefaultValue ?? GetDefaultValue(command.Parameters[i].Type);
        }

        // Parse options first, collecting remaining arguments
        var remainingArgs = new List<string>();
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var isOption = false;

            if (arg.StartsWith("--"))
            {
                // Handle long option format: --option-name
                var longName = arg.Substring(2);
                var option = options.FirstOrDefault(o => o.LongName == longName);
                if (option != null)
                {
                    isOption = true;
                    var optionIndex = Array.IndexOf(command.Parameters, option);
                    SetOptionValue(result, optionIndex, option, args, ref i);
                }
            }
            else if (arg.StartsWith("-") && arg.Length == 2)
            {
                // Handle short option format: -o
                var shortName = arg.Substring(1);
                var option = options.FirstOrDefault(o => o.ShortName == shortName);
                if (option != null)
                {
                    isOption = true;
                    var optionIndex = Array.IndexOf(command.Parameters, option);
                    SetOptionValue(result, optionIndex, option, args, ref i);
                }
            }

            // If not an option, add to remaining arguments for positional processing
            if (!isOption)
            {
                remainingArgs.Add(arg);
            }
        }

        // Process positional arguments in order
        for (var i = 0; i < arguments.Length && i < remainingArgs.Count; i++)
        {
            var argument = arguments[i];
            var argIndex = Array.IndexOf(command.Parameters, argument);
            result[argIndex] = ConvertValue(remainingArgs[i], argument.Type);
        }

        // Validate that all required arguments are provided
        foreach (var arg in arguments)
        {
            if (!arg.Required) continue;
            var argIndex = Array.IndexOf(command.Parameters, arg);
            if (result[argIndex] == null || result[argIndex]!.Equals(GetDefaultValue(arg.Type)))
            {
                throw new ArgumentException($"Required argument '{arg.Name}' is missing");
            }
        }

        return result;
    }

    /// <summary>
    /// Sets the value for an option parameter, handling both boolean flags and value options.
    /// </summary>
    /// <param name="result">Result array to store the parsed value</param>
    /// <param name="index">Index in the result array</param>
    /// <param name="option">Option parameter info</param>
    /// <param name="args">All command-line arguments</param>
    /// <param name="argIndex">Current argument index (passed by reference to advance)</param>
    private static void SetOptionValue(object[] result, int index, ParameterInfo option, string[] args,
        ref int argIndex)
    {
        if (option.Type == typeof(bool))
        {
            // Boolean options are flags - presence means true
            result[index] = true;
        }
        else if (argIndex + 1 < args.Length)
        {
            // Value options require the next argument as the value
            argIndex++;
            result[index] = ConvertValue(args[argIndex], option.Type);
        }
        else
        {
            throw new ArgumentException($"Option {option.LongName} requires a value");
        }
    }

    /// <summary>
    /// Converts a string argument to the target type.
    /// Handles common types, nullable types, and enums.
    /// </summary>
    /// <param name="value">String value to convert</param>
    /// <param name="targetType">Target type to convert to</param>
    /// <returns>Converted value</returns>
    private static object ConvertValue(string value, Type targetType)
    {
        // Handle nullable types by getting the underlying type
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            targetType = Nullable.GetUnderlyingType(targetType)!;
        }

        // Handle common types with specific parsing
        if (targetType == typeof(string)) return value;
        if (targetType == typeof(int)) return int.Parse(value);
        if (targetType == typeof(double)) return double.Parse(value);
        if (targetType == typeof(float)) return float.Parse(value);
        if (targetType == typeof(long)) return long.Parse(value);
        if (targetType == typeof(decimal)) return decimal.Parse(value);
        if (targetType == typeof(bool)) return bool.Parse(value);
        if (targetType.IsEnum) return Enum.Parse(targetType, value, true);

        // Fallback to general type conversion
        return Convert.ChangeType(value, targetType);
    }

    /// <summary>
    /// Gets the default value for the specified type using compiled expression trees.
    /// </summary>
    /// <param name="type">The type to get the default value for.</param>
    /// <returns>
    /// The default value for the specified type:
    /// <list type="bullet">
    /// <item><description>For reference types and nullable value types: <see langword="null"/></description></item>
    /// <item><description>For value types: The result of <c>default(T)</c> (e.g., 0 for <see cref="int"/>, false for <see cref="bool"/>)</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method uses cached compiled expression trees for optimal performance on repeated calls.
    /// The first call for each type compiles an expression equivalent to <c>() => (object)default(T)</c>,
    /// which is then cached and reused for subsequent calls.
    /// </para>
    /// <para>
    /// This approach is trimming-safe and does not rely on reflection or <see cref="Activator.CreateInstance(Type)"/>,
    /// making it suitable for AOT compilation scenarios.
    /// </para>
    /// </remarks>
    private static object GetDefaultValue(Type type)
    {
        var factory = DefaultValueFactories.GetOrAdd(type, t =>
        {
            // Handle nullable and reference types
            if (!t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                return () => null!;
            }

            // Create expression: () => (object)default(T)
            var defaultExpr = Expression.Default(t);
            var convertExpr = Expression.Convert(defaultExpr, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(convertExpr);
            return lambda.Compile();
        });

        return factory();
    }

    /// <summary>
    /// Displays help information for a specific command.
    /// Shows usage, description, arguments, and options.
    /// </summary>
    /// <param name="commandName">Name of the command to show help for</param>
    /// <param name="command">Command info containing help details</param>
    private void ShowCommandHelp(string commandName, CommandInfo command)
    {
        Console.WriteLine($"Usage: {name} {commandName} [OPTIONS] [ARGUMENTS]");
        Console.WriteLine();

        if (!string.IsNullOrEmpty(command.Description))
        {
            Console.WriteLine(command.Description);
            Console.WriteLine();
        }

        // Show arguments section
        var arguments = command.Parameters.Where(p => p.IsArgument).OrderBy(p => p.Position);
        if (arguments.Any())
        {
            Console.WriteLine("Arguments:");
            foreach (var arg in arguments)
            {
                var required = arg.Required ? " (required)" : " (optional)";
                Console.WriteLine($"  {arg.Name.ToUpper()}{required}  {arg.Description}");
            }

            Console.WriteLine();
        }

        // Show options section
        var options = command.Parameters.Where(p => p.IsOption);
        if (options.Any())
        {
            Console.WriteLine("Options:");
            foreach (var opt in options)
            {
                var shortOpt = string.IsNullOrEmpty(opt.ShortName) ? "   " : $"-{opt.ShortName},";
                Console.WriteLine($"  {shortOpt} --{opt.LongName}  {opt.Description}");
            }

            Console.WriteLine();
        }
    }

    /// <summary>
    /// Displays the main help screen showing all available commands.
    /// Called when no command is specified or help is requested.
    /// </summary>
    private void ShowHelp()
    {
        Console.WriteLine($"{name} - {description}");
        Console.WriteLine();
        Console.WriteLine("USAGE:");
        Console.WriteLine($"  {name} <command> [options] [arguments]");
        Console.WriteLine();
        Console.WriteLine("COMMANDS:");

        foreach (var kvp in _commands)
        {
            Console.WriteLine($"  {kvp.Key,-15} {kvp.Value.Description}");
        }

        Console.WriteLine();
        Console.WriteLine($"Use '{name} <command> --help' for more information about a command.");
    }
}