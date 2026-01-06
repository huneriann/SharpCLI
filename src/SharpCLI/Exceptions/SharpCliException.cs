using System;
using System.Runtime.Serialization;

namespace SharpCLI;

/// <summary>
/// Base exception class for all SharpCLI framework-related errors.
/// This allows catching framework-specific exceptions separately from general exceptions.
/// </summary>
[Serializable]
public abstract class SharpCliException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SharpCliException"/> class.
    /// </summary>
    protected SharpCliException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SharpCliException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected SharpCliException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SharpCliException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    protected SharpCliException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SharpCliException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected SharpCliException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}