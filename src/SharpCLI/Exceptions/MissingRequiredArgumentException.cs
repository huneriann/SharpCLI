using System;
using System.Runtime.Serialization;

namespace SharpCLI;

/// <summary>
/// Thrown when a required argument is missing from the command-line input.
/// </summary>
[Serializable]
public class MissingRequiredArgumentException : ArgumentParsingException
{
    /// <summary>
    /// Gets the name of the missing required argument.
    /// </summary>
    public string ArgumentName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingRequiredArgumentException"/> class.
    /// </summary>
    public MissingRequiredArgumentException() : base()
    {
        ArgumentName = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingRequiredArgumentException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected MissingRequiredArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        ArgumentName = info.GetString(nameof(ArgumentName)) ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingRequiredArgumentException"/> class with the specified argument name.
    /// </summary>
    /// <param name="argumentName">The name of the missing required argument.</param>
    public MissingRequiredArgumentException(string argumentName)
        : base($"Required argument '{argumentName}' is missing.")
    {
        ArgumentName = argumentName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingRequiredArgumentException"/> class with the specified argument name and inner exception.
    /// </summary>
    /// <param name="argumentName">The name of the missing required argument.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MissingRequiredArgumentException(string argumentName, Exception innerException)
        : base($"Required argument '{argumentName}' is missing.", innerException)
    {
        ArgumentName = argumentName;
    }
}