using System;
using System.Runtime.Serialization;

namespace SharpCLI;

/// <summary>
/// Thrown when attempting to register a command that already exists in the host.
/// </summary>
[Serializable]
public class CommandAlreadyExistsException : SharpCliException
{
    /// <summary>
    /// Gets the name of the command that already exists.
    /// </summary>
    public string CommandName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAlreadyExistsException"/> class.
    /// </summary>
    public CommandAlreadyExistsException() : base()
    {
        CommandName = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAlreadyExistsException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected CommandAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        CommandName = info.GetString(nameof(CommandName)) ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAlreadyExistsException"/> class with the specified command name.
    /// </summary>
    /// <param name="commandName">The name of the command that already exists.</param>
    public CommandAlreadyExistsException(string commandName)
        : base($"A command with the name '{commandName}' is already registered.")
    {
        CommandName = commandName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAlreadyExistsException"/> class with the specified command name and inner exception.
    /// </summary>
    /// <param name="commandName">The name of the command that already exists.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CommandAlreadyExistsException(string commandName, Exception innerException)
        : base($"A command with the name '{commandName}' is already registered.", innerException)
    {
        CommandName = commandName;
    }
}