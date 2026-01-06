using System;
using System.Runtime.Serialization;

namespace SharpCLI;

/// <summary>
/// Thrown when a command is not found during execution.
/// </summary>
[Serializable]
public class CommandNotFoundException : SharpCliException
{
    /// <summary>
    /// Gets the name of the command that was not found.
    /// </summary>
    public string CommandName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandNotFoundException"/> class.
    /// </summary>
    public CommandNotFoundException() : base()
    {
        CommandName = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandNotFoundException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected CommandNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        CommandName = info.GetString(nameof(CommandName)) ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandNotFoundException"/> class with the specified command name.
    /// </summary>
    /// <param name="commandName">The name of the command that was not found.</param>
    public CommandNotFoundException(string commandName)
        : base($"Command '{commandName}' not found. Use '--help' for available commands.")
    {
        CommandName = commandName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandNotFoundException"/> class with the specified command name and inner exception.
    /// </summary>
    /// <param name="commandName">The name of the command that was not found.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CommandNotFoundException(string commandName, Exception innerException)
        : base($"Command '{commandName}' not found. Use '--help' for available commands.", innerException)
    {
        CommandName = commandName;
    }
}