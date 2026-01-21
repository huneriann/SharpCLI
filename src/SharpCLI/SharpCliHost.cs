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
public sealed class SharpCliHost : IDisposable
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
    /// .Name("myapp")
    /// .Description("My awesome CLI tool")
    /// .CustomHelpMessage("Type 'myapp &lt;command&gt; --help' for details.")
    /// .Build();
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
    /// This acts as the high-level orchestrator for command registration.
    /// </summary>
    /// <param name="instance">The object instance containing the method (null for static methods).</param>
    /// <param name="method">The reflection metadata of the method to register.</param>
    /// <param name="commandAttr">The command attribute containing name and alias metadata.</param>
    private void RegisterCommand(object? instance, MethodInfo method, CommandAttribute commandAttr)
    {
        var parameterInfos = ExtractParameters(method);
        ValidateParameterNames(parameterInfos);
        var isAsync = ValidateAndCheckAsync(method);
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
        StoreCommand(commandInfo);
    }

    /// <summary>
    /// Iterates through method parameters and converts them into the internal ParameterInfo model.
    /// </summary>
    /// <param name="method">The method to reflect for parameters.</param>
    /// <returns>A list of processed parameter metadata.</returns>
    private static List<ParameterInfo> ExtractParameters(MethodInfo method)
    {
        var parameterInfos = new List<ParameterInfo>();
        foreach (var param in method.GetParameters())
        {
            var currentArgCount = parameterInfos.Count(p => p.IsArgument);
            parameterInfos.Add(CreateParameterInfo(param, currentArgCount));
        }

        return parameterInfos;
    }

    /// <summary>
    /// Determines the type of parameter based on attributes and creates the appropriate info object.
    /// </summary>
    /// <param name="param">The reflection parameter metadata.</param>
    /// <param name="currentArgCount">The current count of positional arguments for auto-positioning.</param>
    /// <returns>A mapped ParameterInfo object.</returns>
    private static ParameterInfo CreateParameterInfo(System.Reflection.ParameterInfo param, int currentArgCount)
    {
        var argAttr = param.GetCustomAttribute<ArgumentAttribute>();
        var optAttr = param.GetCustomAttribute<OptionAttribute>();
        if (argAttr != null)
        {
            return MapArgument(param, argAttr, currentArgCount);
        }

        if (optAttr != null)
        {
            return MapOption(param, optAttr);
        }

        return MapDefaultArgument(param, currentArgCount);
    }

    /// <summary>
    /// Maps a parameter explicitly marked with the [Argument] attribute.
    /// </summary>
    private static ParameterInfo MapArgument(System.Reflection.ParameterInfo param, ArgumentAttribute attr,
        int position)
    {
        return new ParameterInfo
        {
            Name = attr.Name,
            Type = param.ParameterType,
            IsArgument = true,
            Required = attr.Required,
            Description = attr.Description,
            Position = position
        };
    }

    /// <summary>
    /// Maps a parameter explicitly marked with the [Option] attribute.
    /// </summary>
    private static ParameterInfo MapOption(System.Reflection.ParameterInfo param, OptionAttribute attr)
    {
        return new ParameterInfo
        {
            Name = param.Name!,
            Type = param.ParameterType,
            IsOption = true,
            Required = !param.HasDefaultValue,
            DefaultValue = attr.DefaultValue ??
                           (param.HasDefaultValue ? param.DefaultValue : GetDefaultValue(param.ParameterType)),
            Description = attr.Description,
            ShortName = attr.ShortName,
            LongName = attr.LongName
        };
    }

    /// <summary>
    /// Maps a parameter without attributes, defaulting it to a positional argument.
    /// </summary>
    private static ParameterInfo MapDefaultArgument(System.Reflection.ParameterInfo param, int position)
    {
        return new ParameterInfo
        {
            Name = param.Name!,
            Type = param.ParameterType,
            IsArgument = true,
            Required = !param.HasDefaultValue,
            DefaultValue = param.HasDefaultValue ? param.DefaultValue : GetDefaultValue(param.ParameterType),
            Position = position
        };
    }

    /// <summary>
    /// Validates that no two parameters share the same name within a single command.
    /// </summary>
    /// <param name="parameters">The list of parameters to check.</param>
    /// <exception cref="InvalidCommandConfigurationException">Thrown if duplicates are found.</exception>
    private static void ValidateParameterNames(List<ParameterInfo> parameters)
    {
        var duplicateNames = parameters
            .GroupBy(p => p.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateNames.Any())
        {
            throw new InvalidCommandConfigurationException(
                $"Duplicate parameter names found: {string.Join(", ", duplicateNames)}");
        }
    }

    /// <summary>
    /// Validates the method return type and determines if the command execution is asynchronous.
    /// </summary>
    /// <param name="method">The method to validate.</param>
    /// <returns>True if the method returns a Task; otherwise false.</returns>
    /// <exception cref="InvalidCommandConfigurationException">Thrown if the return type is not supported.</exception>
    private static bool ValidateAndCheckAsync(MethodInfo method)
    {
        var type = method.ReturnType;
        var isAsync = type == typeof(Task<int>) || type == typeof(Task);
        var isSync = type == typeof(int) || type == typeof(void);
        if (!isAsync && !isSync)
        {
            throw new InvalidCommandConfigurationException(
                $"Unsupported return type '{type.Name}' for command '{method.Name}'.");
        }

        return isAsync;
    }

    /// <summary>
    /// Persists the command and its aliases into the host's internal thread-safe dictionaries.
    /// </summary>
    /// <param name="commandInfo">The fully constructed command metadata.</param>
    /// <exception cref="CommandAlreadyExistsException">Thrown if the command name is already registered.</exception>
    /// <exception cref="AliasAlreadyExistsException">Thrown if an alias is already in use.</exception>
    private void StoreCommand(CommandInfo commandInfo)
    {
        if (!_commands.TryAdd(commandInfo.Name, commandInfo))
        {
            throw new CommandAlreadyExistsException(commandInfo.Name);
        }

        // Use .Where to identify aliases that are already registered
        var conflictingAliases = commandInfo.Aliases
            .Where(alias => _aliases.ContainsKey(alias))
            .ToList();
        if (conflictingAliases.Count > 0)
        {
            throw new AliasAlreadyExistsException(conflictingAliases[0]);
        }

        // Now perform the side effect (adding to dictionary)
        foreach (var alias in commandInfo.Aliases)
        {
            _aliases.TryAdd(alias, commandInfo.Name);
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
        if (args.Length == 0)
        {
            await ShowHelpAsync();
            return 0;
        }

        var commandName = args[0];

        if (IsGlobalHelp(commandName))
        {
            await ShowHelpAsync();
            return 0;
        }

        commandName = ResolveAlias(commandName);

        var command = GetCommand(commandName);

        var commandArgs = args.Skip(1).ToArray();

        return await ExecuteCommand(command, commandArgs);
    }

    private bool IsGlobalHelp(string commandName) => commandName is "--help" or "-h" or "help";

    private string ResolveAlias(string commandName)
    {
        return _aliases.GetValueOrDefault(commandName, commandName);
    }

    private CommandInfo GetCommand(string commandName)
    {
        if (!_commands.TryGetValue(commandName, out var command))
        {
            throw new CommandNotFoundException(commandName);
        }

        return command!;
    }

    private async Task<int> ExecuteCommand(CommandInfo command, string[] commandArgs)
    {
        try
        {
            if (commandArgs.Contains("--help") || commandArgs.Contains("-h"))
            {
                await ShowCommandHelp(command.Name, command);
                return 0;
            }

            var parameters = ParseArguments(command, commandArgs);

            return await InvokeCommand(command, parameters);
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
            var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            await _writer.WriteLineAsync($"Error executing command: {message}");
            return 1;
        }
    }

    private async Task<int> InvokeCommand(CommandInfo command, object[] parameters)
    {
        var result = command.Method.Invoke(command.Instance, parameters);

        if (command.IsAsync)
        {
            switch (result)
            {
                case Task<int> taskInt:
                    return await taskInt;
                case Task task:
                    await task;
                    return 0;
                default:
                    return 0;
            }
        }
        else
        {
            if (result is int intResult)
            {
                return intResult;
            }

            return 0;
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
        var arguments = command.Parameters.Where(p => p.IsArgument).OrderBy(p => p.Position).ToArray();
        var options = command.Parameters.Where(p => p.IsOption).ToArray();
        var result = new object[command.Parameters.Length];

        InitializeDefaults(command.Parameters, result);

        var remainingArgs = ProcessOptions(args, options, command.Parameters, result);

        ProcessPositionalArguments(arguments, remainingArgs, command.Parameters, result);

        ValidateRequiredArguments(arguments, command.Parameters, result);

        return result;
    }

    private static void InitializeDefaults(ParameterInfo[] parameters, object[] result)
    {
        for (var i = 0; i < parameters.Length; i++)
        {
            result[i] = parameters[i].DefaultValue ?? GetDefaultValue(parameters[i].Type);
        }
    }

    private static List<string> ProcessOptions(string[] args, ParameterInfo[] options, ParameterInfo[] allParameters,
        object[] result)
    {
        var remainingArgs = new List<string>();

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var isOption = false;
            var isNegativeNumber = arg.Length > 1 && arg[0] == '-' && char.IsDigit(arg[1]);

            if (!isNegativeNumber)
            {
                if (arg.StartsWith("--"))
                {
                    var longName = arg.Substring(2);
                    var option = options.FirstOrDefault(o => o.LongName == longName);
                    if (option != null)
                    {
                        isOption = true;
                        var optionIndex = Array.IndexOf(allParameters, option);
                        SetOptionValue(result, optionIndex, option, args, ref i);
                    }
                }
                else if (arg.StartsWith("-") && arg.Length >= 2)
                {
                    var shortName = arg.Substring(1);
                    var option = options.FirstOrDefault(o => o.ShortName == shortName);
                    if (option != null)
                    {
                        isOption = true;
                        var optionIndex = Array.IndexOf(allParameters, option);
                        SetOptionValue(result, optionIndex, option, args, ref i);
                    }
                }
            }

            if (isOption) continue;

            if (arg.StartsWith("-") && !isNegativeNumber)
            {
                throw new UnrecognizedArgumentException(arg);
            }

            remainingArgs.Add(arg);
        }

        return remainingArgs;
    }

    private static void ProcessPositionalArguments(ParameterInfo[] arguments, List<string> remainingArgs,
        ParameterInfo[] allParameters, object[] result)
    {
        for (var i = 0; i < arguments.Length && i < remainingArgs.Count; i++)
        {
            var argument = arguments[i];
            var argIndex = Array.IndexOf(allParameters, argument);
            result[argIndex] = ConvertValue(remainingArgs[i], argument.Type, argument.Name);
        }

        if (remainingArgs.Count > arguments.Length)
        {
            throw new UnrecognizedArgumentException(string.Join(" ", remainingArgs.Skip(arguments.Length)));
        }
    }

    private static void ValidateRequiredArguments(ParameterInfo[] arguments, ParameterInfo[] allParameters,
        object[] result)
    {
        foreach (var arg in arguments)
        {
            if (!arg.Required) continue;
            var argIndex = Array.IndexOf(allParameters, arg);
            if (Equals(result[argIndex], GetDefaultValue(arg.Type)))
            {
                throw new MissingRequiredArgumentException(arg.Name);
            }
        }
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
            var description = string.IsNullOrWhiteSpace(item.Value.Description) ? "" : $" | {item.Value.Description}";
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