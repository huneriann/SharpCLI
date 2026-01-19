namespace SharpCLI;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Text;
using System.Reflection;
using Models;
using ParameterInfo = Models.ParameterInfo;

/// <summary>
/// Main host class for SharpCli framework that manages CLI commands, arguments, and options.
/// Provides an attribute-based approach for building command-line applications in C#.
/// </summary>
public class SharpCliHost : IDisposable
{
    /// <summary>
    /// Dictionary storing all registered commands by their names
    /// </summary>
    private readonly ConcurrentDictionary<string, CommandInfo> _commands = new();

    /// <summary>
    /// Dictionary mapping command aliases to their actual command names
    /// </summary>
    private readonly ConcurrentDictionary<string, string> _aliases = new();

    /// <summary>
    /// Cache of compiled factory functions that generate default values for types.
    /// This provides high-performance type-safe default value creation while avoiding
    /// trimming issues with Activator.CreateInstance.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, Func<object>> DefaultValueFactories = new();

    /// <summary>
    /// The name of the CLI application used in help headers and usage strings.
    /// </summary>
    private readonly string _name;

    /// <summary>
    /// A brief description of the CLI application displayed in the main help output.
    /// </summary>
    private readonly string _description;

    /// <summary>
    /// An optional user-defined message displayed at the top of the help screen. 
    /// If provided, this replaces the default help header.
    /// </summary>
    private readonly string? _customHelpMessage;

    /// <summary>
    /// The text writer destination for all output (commands, errors, and help). 
    /// Usually defaults to <see cref="Console.Out"/>.
    /// </summary>
    private readonly TextWriter _writer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SharpCliHost"/> class.
    /// </summary>
    /// <param name="name">The name of the CLI application. This is displayed in help output and usage messages.</param>
    /// <param name="description">Optional description of the CLI application, shown in the main help screen.</param>
    /// <param name="writer">
    /// The <see cref="TextWriter"/> used for all output, including help text and error messages.
    /// Defaults to <see cref="Console.Out"/> if null. Injecting a custom writer is recommended for unit testing.
    /// </param>
    /// <param name="customHelpMessage">
    /// Optional custom message to display at the top of the main help screen (when no command is provided or --help is used).
    /// If provided, this message is shown before the standard usage and command list.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
    public SharpCliHost(string name, string description = "", TextWriter? writer = null, string? customHelpMessage = "")
    {
        _name = name;
        _description = description;
        _customHelpMessage = customHelpMessage;
        _writer = writer ?? Console.Out;
    }

    /// <summary>
    /// Creates a new builder instance to configure and construct a <see cref="SharpCliHost"/> using a fluent API.
    /// </summary>
    /// <returns>A new <see cref="SharpCliHostBuilder"/> instance for configuring the host.</returns>
    /// <example>
    /// <code>
    /// var host = SharpCliHost.CreateBuilder()
    ///     .Name("myapp")
    ///     .Description("My awesome CLI tool")
    ///     .CustomHelpMessage("Type 'myapp &lt;command&gt; --help' for details.")
    ///     .Build();
    /// </code>
    /// </example>
    public static SharpCliHostBuilder CreateBuilder() => new();

    /// <summary>
    /// Registers all methods marked with [Command] attribute from an instance object.
    /// This allows for instance-based commands where the object can maintain state.
    /// </summary>
    /// <param name="commandsContainer">Container containing command methods</param>
    /// <returns>Current SharpCliHost instance for method chaining</returns>
    public SharpCliHost RegisterCommands(ICommandsContainer commandsContainer)
    {
        var type = commandsContainer.GetType();
        // Get both instance and static methods from the object
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        foreach (var method in methods)
        {
            var commandAttr = method.GetCustomAttribute<CommandAttribute>();
            if (commandAttr != null)
            {
                RegisterCommand(commandsContainer, method, commandAttr);
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
    public SharpCliHost RegisterCommands<T>() where T : class, ICommandsContainer
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
                if (param.DefaultValue != null)
                    parameterInfos.Add(new ParameterInfo
                    {
                        Name = param.Name!,
                        Type = param.ParameterType,
                        IsOption = true,
                        Required = !param.HasDefaultValue,
                        DefaultValue = optAttr.DefaultValue ??
                                       (param.HasDefaultValue
                                           ? param.DefaultValue
                                           : GetDefaultValue(param.ParameterType)),
                        Description = optAttr.Description,
                        ShortName = optAttr.ShortName,
                        LongName = optAttr.LongName
                    });
            }
            else
            {
                // Parameter without attribute - treat as positional argument
                if (param.DefaultValue != null)
                    parameterInfos.Add(new ParameterInfo
                    {
                        Name = param.Name!,
                        Type = param.ParameterType,
                        IsArgument = true,
                        Required = !param.HasDefaultValue,
                        DefaultValue =
                            param.HasDefaultValue ? param.DefaultValue : GetDefaultValue(param.ParameterType),
                        Position = parameterInfos.Count(p => p.IsArgument)
                    });
            }
        }

        // Check for duplicate parameter names
        var duplicateNames = parameterInfos
            .GroupBy(p => p.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateNames.Any())
        {
            throw new InvalidCommandConfigurationException(
                $"Duplicate parameter names found: {string.Join(", ", duplicateNames)}");
        }

        // Determine if the method is async or sync and validate return type
        var returnType = method.ReturnType;
        var isAsync = returnType == typeof(Task<int>) || returnType == typeof(Task);
        var isSync = returnType == typeof(int) || returnType == typeof(void);
        if (!isAsync && !isSync)
        {
            throw new InvalidCommandConfigurationException(
                $"Unsupported return type '{returnType.Name}' for command method '{method.Name}'. Supported: Task<int>, Task, int, void.");
        }

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
        if (!_commands.TryAdd(commandInfo.Name, commandInfo))
        {
            throw new CommandAlreadyExistsException(commandInfo.Name);
        }

        // Register all aliases for this command
        foreach (var alias in commandAttr.Aliases)
        {
            if (!_aliases.TryAdd(alias, commandAttr.Name))
            {
                throw new AliasAlreadyExistsException(alias);
            }
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
            await ShowHelpAsync();
            return 0;
        }

        var commandName = args[0];

        // Handle global help commands
        if (commandName is "--help" or "-h" or "help")
        {
            await ShowHelpAsync();
            return 0;
        }

        // Resolve command aliases to actual command names
        if (_aliases.TryGetValue(commandName, out var alias))
        {
            commandName = alias;
        }

        // Check if command exists
        if (!_commands.TryGetValue(commandName, out var command))
        {
            throw new CommandNotFoundException(commandName);
        }

        // Remove command name from arguments
        var commandArgs = args.Skip(1).ToArray();

        try
        {
            // Handle command-specific help
            if (commandArgs.Contains("--help") || commandArgs.Contains("-h"))
            {
                await ShowCommandHelp(commandName, command);
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
                    // Return the int result from Task<int>
                    case Task<int> taskInt:
                        return await taskInt;
                    // Task without return value, assume success
                    case Task task:
                        await task;
                        return 0;
                    // Fallback for unexpected return types
                    default:
                        return 0;
                }
            }
            else
            {
                // Handle synchronous methods
                var result = command.Method.Invoke(command.Instance, parameters);
                if (result is int intResult) return intResult; // Return the int result
                // Void methods or unexpected return types, assume success
                return 0;
            }
        }
        catch (SharpCliException ex)
        {
            await _writer.WriteLineAsync($"Error: {ex.Message}");
            if (ex is CommandNotFoundException)
            {
                await _writer.WriteLineAsync($"Use '{_name} --help' for usage information.");
            }

            return 1;
        }
        catch (Exception ex)
        {
            // Handle any errors during command execution
            if (ex.InnerException != null)
            {
                await _writer.WriteLineAsync($"Error executing command: {ex.InnerException.Message}");
                return 1;
            }

            await _writer.WriteLineAsync($"Error executing command: {ex.Message}");
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
    private static object[] ParseArguments(CommandInfo command, string[] args)
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

        var remainingArgs = new List<string>();
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var isOption = false;

            // Check if the argument is a negative number instead of an option flag
            // A negative number starts with '-' and is followed by a digit (e.g., -5, -0.1)
            var isNegativeNumber = arg.Length > 1 && arg[0] == '-' && char.IsDigit(arg[1]);

            if (!isNegativeNumber)
            {
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
                else if (arg.StartsWith("-") && arg.Length >= 2)
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
            }

            if (isOption) continue;

            // If it starts with '-' but wasn't a recognized option or a negative number, it's invalid
            if (arg.StartsWith("-") && !isNegativeNumber)
            {
                throw new UnrecognizedArgumentException(arg);
            }

            remainingArgs.Add(arg);
        }

        // Process positional arguments in order
        for (var i = 0; i < arguments.Length && i < remainingArgs.Count; i++)
        {
            var argument = arguments[i];
            var argIndex = Array.IndexOf(command.Parameters, argument);
            result[argIndex] = ConvertValue(remainingArgs[i], argument.Type, argument.Name);
        }

        // Check for extra unrecognized positional arguments
        if (remainingArgs.Count > arguments.Length)
        {
            throw new UnrecognizedArgumentException(string.Join(" ", remainingArgs.Skip(arguments.Length)));
        }

        // Validate that all required arguments are provided
        foreach (var arg in arguments)
        {
            if (!arg.Required) continue;
            var argIndex = Array.IndexOf(command.Parameters, arg);

            // Check if the result still holds the default value (meaning it wasn't provided)
            if (Equals(result[argIndex], GetDefaultValue(arg.Type)))
            {
                throw new MissingRequiredArgumentException(arg.Name);
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
            result[index] = ConvertValue(args[argIndex], option.Type, option.Name);
        }
        else
        {
            throw new MissingOptionValueException(option.LongName);
        }
    }

    /// <summary>
    /// Converts a string argument to the target type.
    /// Handles common types, nullable types, and enums.
    /// </summary>
    /// <param name="value">String value to convert</param>
    /// <param name="targetType">Target type to convert to</param>
    /// <param name="paramName">Name of the parameter for error reporting</param>
    /// <returns>Converted value</returns>
    private static object ConvertValue(string value, Type targetType, string paramName)
    {
        try
        {
            // Handle nullable types by getting the underlying type
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // Handle common types with specific parsing
            if (underlyingType == typeof(string)) return value;
            if (underlyingType == typeof(int)) return int.Parse(value, CultureInfo.InvariantCulture);
            if (underlyingType == typeof(double)) return double.Parse(value, CultureInfo.InvariantCulture);
            if (underlyingType == typeof(float)) return float.Parse(value, CultureInfo.InvariantCulture);
            if (underlyingType == typeof(long)) return long.Parse(value, CultureInfo.InvariantCulture);
            if (underlyingType == typeof(decimal)) return decimal.Parse(value, CultureInfo.InvariantCulture);
            if (underlyingType == typeof(bool)) return bool.Parse(value);
            if (underlyingType.IsEnum) return Enum.Parse(underlyingType, value, true);

            // Fallback to general type conversion
            return Convert.ChangeType(value, underlyingType);
        }
        catch (Exception ex)
        {
            throw new InvalidArgumentValueException(paramName, value, targetType, ex);
        }
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
    private async Task ShowCommandHelp(string commandName, CommandInfo command)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Usage: {_name} {commandName} [OPTIONS] [ARGUMENTS]\n");
        if (!string.IsNullOrEmpty(command.Description))
        {
            sb.AppendLine($"{command.Description}");
        }

        // Show arguments section
        var arguments = command
            .Parameters
            .Where(p => p.IsArgument)
            .OrderBy(p => p.Position)
            .ToList();

        if (arguments.Any())
        {
            sb.AppendLine("\nArguments:");
            foreach (var arg in arguments)
            {
                var required = arg.Required ? " (required)" : " (optional)";
                sb.AppendLine($" {arg.Name.ToUpper()}{required} | {arg.Description}");
            }
        }

        // Show options section
        var options = command
            .Parameters
            .Where(p => p.IsOption)
            .ToList();

        if (options.Any())
        {
            sb.AppendLine("\nOptions:");
            foreach (var opt in options)
            {
                var shortOpt = string.IsNullOrEmpty(opt.ShortName) ? " " : $"-{opt.ShortName},";
                sb.AppendLine($" {shortOpt} --{opt.LongName} | {opt.Description}");
            }
        }

        await _writer.WriteLineAsync(sb.ToString());
    }

    /// <summary>
    /// Displays the main help screen showing all available commands.
    /// Called when no command is specified or help is requested.
    /// </summary>
    private async Task ShowHelpAsync()
    {
        if (!string.IsNullOrWhiteSpace(_customHelpMessage))
        {
            await _writer.WriteLineAsync(_customHelpMessage);
            return;
        }

        if (!string.IsNullOrWhiteSpace(_description)) await _writer.WriteLineAsync($"{_name} - {_description}\n");
        else await _writer.WriteLineAsync($"{_name}\n");

        await _writer.WriteLineAsync("USAGE:\n");
        await _writer.WriteLineAsync($" {_name} <command> [options] [arguments]\n");

        if (!_commands.IsEmpty) await _writer.WriteLineAsync("COMMANDS:");

        foreach (var item in _commands.OrderBy(kvp => kvp.Key))
        {
            var description = string.IsNullOrWhiteSpace(item.Value.Description)
                ? ""
                : $" | {item.Value.Description}";

            await _writer.WriteAsync($"\t{item.Key}{description}\n");
        }

        await _writer.WriteLineAsync($"\nUse '{_name} <command> --help' for more information about a command.");
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting
    /// unmanaged resources synchronously.
    /// </summary>
    /// <remarks>
    /// Disposes of the underlying <see cref="TextWriter"/> if it was provided.
    /// </remarks>
    public void Dispose()
    {
        _writer.Dispose();
    }
}