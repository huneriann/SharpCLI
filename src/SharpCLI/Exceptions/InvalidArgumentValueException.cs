using System;
using System.Runtime.Serialization;

namespace SharpCLI;

/// <summary>
/// Thrown when an argument or option value cannot be converted to the expected type.
/// </summary>
[Serializable]
public class InvalidArgumentValueException : ArgumentParsingException
{
    /// <summary>
    /// Gets the name of the argument or option with the invalid value.
    /// </summary>
    public string ArgumentName { get; }

    /// <summary>
    /// Gets the provided invalid value.
    /// </summary>
    public string ProvidedValue { get; }

    /// <summary>
    /// Gets the expected type for the argument or option.
    /// </summary>
    public Type ExpectedType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidArgumentValueException"/> class.
    /// </summary>
    public InvalidArgumentValueException() : base()
    {
        ArgumentName = string.Empty;
        ProvidedValue = string.Empty;
        ExpectedType = typeof(object);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidArgumentValueException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidArgumentValueException(string message) : base(message)
    {
        ArgumentName = string.Empty;
        ProvidedValue = string.Empty;
        ExpectedType = typeof(object);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidArgumentValueException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public InvalidArgumentValueException(string message, Exception innerException) : base(message, innerException)
    {
        ArgumentName = string.Empty;
        ProvidedValue = string.Empty;
        ExpectedType = typeof(object);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidArgumentValueException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected InvalidArgumentValueException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        ArgumentName = info.GetString(nameof(ArgumentName)) ?? string.Empty;
        ProvidedValue = info.GetString(nameof(ProvidedValue)) ?? string.Empty;
        ExpectedType = (Type)info.GetValue(nameof(ExpectedType), typeof(Type)) ?? typeof(object);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidArgumentValueException"/> class with the specified details.
    /// </summary>
    /// <param name="argumentName">The name of the argument or option.</param>
    /// <param name="providedValue">The provided invalid value.</param>
    /// <param name="expectedType">The expected type.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or null.</param>
    public InvalidArgumentValueException(string argumentName, string providedValue, Type expectedType,
        Exception? innerException = null)
        : base($"Invalid value '{providedValue}' for '{argumentName}'. Expected type: {expectedType.Name}.",
            innerException)
    {
        ArgumentName = argumentName;
        ProvidedValue = providedValue;
        ExpectedType = expectedType;
    }
}