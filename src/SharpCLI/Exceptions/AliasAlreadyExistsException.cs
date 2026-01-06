using System;
using System.Runtime.Serialization;

namespace SharpCLI;

/// <summary>
/// Thrown when attempting to register an alias that is already in use for another command.
/// </summary>
[Serializable]
public class AliasAlreadyExistsException : SharpCliException
{
    /// <summary>
    /// Gets the alias that is already in use.
    /// </summary>
    public string Alias { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AliasAlreadyExistsException"/> class.
    /// </summary>
    public AliasAlreadyExistsException() : base()
    {
        Alias = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AliasAlreadyExistsException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected AliasAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Alias = info.GetString(nameof(Alias)) ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AliasAlreadyExistsException"/> class with the specified alias.
    /// </summary>
    /// <param name="alias">The alias that is already in use.</param>
    public AliasAlreadyExistsException(string alias)
        : base($"An alias '{alias}' is already registered for another command.")
    {
        Alias = alias;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AliasAlreadyExistsException"/> class with the specified alias and inner exception.
    /// </summary>
    /// <param name="alias">The alias that is already in use.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AliasAlreadyExistsException(string alias, Exception innerException)
        : base($"An alias '{alias}' is already registered for another command.", innerException)
    {
        Alias = alias;
    }
}