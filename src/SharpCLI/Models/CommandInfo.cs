namespace SharpCLI.Models;

using System.Reflection;

/// <summary>
/// Represents metadata and execution information for a CLI command.
/// This class stores all necessary information to execute a command method,
/// including its parameters, method reference, and associated metadata.
/// </summary>
internal class CommandInfo
{
    /// <summary>
    /// Gets or sets the primary name of the command as it appears in the CLI.
    /// This is the main identifier used to invoke the command.
    /// </summary>
    /// <example>
    /// For a command "build", users would type: myapp build [options]
    /// </example>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the human-readable description of what the command does.
    /// This description is displayed in help text and command listings.
    /// </summary>
    /// <example>
    /// "Builds the project and outputs to the specified directory"
    /// </example>
    public string Description { get; set; } = "";

    /// <summary>
    /// Gets or sets an array of alternative names (aliases) for the command.
    /// Allows users to invoke the same command using shorter or alternative names.
    /// </summary>
    /// <example>
    /// A "build" command might have aliases ["b", "compile"] allowing:
    /// myapp build, myapp b, or myapp compile
    /// </example>
    public string[] Aliases { get; set; } = [];

    /// <summary>
    /// Gets or sets the MethodInfo that represents the actual method to be executed.
    /// This contains reflection information about the command method including
    /// its parameters, return type, and execution details.
    /// </summary>
    /// <remarks>
    /// Used by the framework to invoke the command method with parsed arguments.
    /// </remarks>
    public MethodInfo Method { get; set; } = null!;

    /// <summary>
    /// Gets or sets the instance object that contains the command method.
    /// This is null for static methods and contains the object reference for instance methods.
    /// </summary>
    /// <remarks>
    /// When commands are registered from a class instance, this property holds
    /// the reference needed to invoke instance methods. For static methods,
    /// this remains null as no instance is required for execution.
    /// </remarks>
    public object? Instance { get; set; } = null;

    /// <summary>
    /// Gets or sets an array of parameter information for the command method.
    /// Each element contains metadata about arguments and options including
    /// their types, default values, and parsing requirements.
    /// </summary>
    /// <remarks>
    /// This array is used during argument parsing to match command-line input
    /// to the appropriate method parameters in the correct order and format.
    /// </remarks>
    public ParameterInfo[] Parameters { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the command method is asynchronous.
    /// True if the method returns Task&lt;int&gt; or Task, false for synchronous methods.
    /// </summary>
    /// <remarks>
    /// This flag determines how the framework executes the command:
    /// - Async methods are awaited using Task execution
    /// - Sync methods are invoked directly
    /// </remarks>
    public bool IsAsync { get; set; }
}