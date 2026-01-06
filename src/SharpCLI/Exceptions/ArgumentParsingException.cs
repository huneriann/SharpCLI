using System;
using System.Runtime.Serialization;

namespace SharpCLI;

/// <summary>
/// Base exception for errors during argument and option parsing.
/// </summary>
[Serializable]
public class ArgumentParsingException : SharpCliException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentParsingException"/> class.
    /// </summary>
    public ArgumentParsingException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentParsingException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ArgumentParsingException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentParsingException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ArgumentParsingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentParsingException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected ArgumentParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}