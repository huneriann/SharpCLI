using System;
using System.Runtime.Serialization;

namespace SharpCLI;

/// <summary>
/// Thrown when a required option value is missing from the command-line input.
/// </summary>
[Serializable]
public class MissingOptionValueException : ArgumentParsingException
{
    /// <summary>
    /// Gets the name of the option that requires a value.
    /// </summary>
    public string OptionName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingOptionValueException"/> class.
    /// </summary>
    public MissingOptionValueException() : base()
    {
        OptionName = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingOptionValueException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected MissingOptionValueException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        OptionName = info.GetString(nameof(OptionName)) ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingOptionValueException"/> class with the specified option name.
    /// </summary>
    /// <param name="optionName">The name of the option that requires a value.</param>
    public MissingOptionValueException(string optionName)
        : base($"Option '{optionName}' requires a value but none was provided.")
    {
        OptionName = optionName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingOptionValueException"/> class with the specified option name and inner exception.
    /// </summary>
    /// <param name="optionName">The name of the option that requires a value.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MissingOptionValueException(string optionName, Exception innerException)
        : base($"Option '{optionName}' requires a value but none was provided.", innerException)
    {
        OptionName = optionName;
    }
}