using System;
using System.Runtime.Serialization;

namespace SharpCLI;

/// <summary>
/// Thrown when there are extra unrecognized arguments provided on the command line.
/// </summary>
[Serializable]
public class UnrecognizedArgumentException : ArgumentParsingException
{
    /// <summary>
    /// Gets the unrecognized argument or option.
    /// </summary>
    public string UnrecognizedArg { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnrecognizedArgumentException"/> class.
    /// </summary>
    public UnrecognizedArgumentException() : base()
    {
        UnrecognizedArg = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnrecognizedArgumentException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected UnrecognizedArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        UnrecognizedArg = info.GetString(nameof(UnrecognizedArg)) ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnrecognizedArgumentException"/> class with the specified unrecognized argument.
    /// </summary>
    /// <param name="unrecognizedArg">The unrecognized argument or option.</param>
    public UnrecognizedArgumentException(string unrecognizedArg)
        : base($"Unrecognized argument or option: '{unrecognizedArg}'.")
    {
        UnrecognizedArg = unrecognizedArg;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnrecognizedArgumentException"/> class with the specified unrecognized argument and inner exception.
    /// </summary>
    /// <param name="unrecognizedArg">The unrecognized argument or option.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UnrecognizedArgumentException(string unrecognizedArg, Exception innerException)
        : base($"Unrecognized argument or option: '{unrecognizedArg}'.", innerException)
    {
        UnrecognizedArg = unrecognizedArg;
    }
}