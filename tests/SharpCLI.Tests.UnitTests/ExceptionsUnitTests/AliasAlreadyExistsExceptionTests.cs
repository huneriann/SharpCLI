namespace SharpCLI.Tests.UnitTests.ExceptionsUnitTests;

public class AliasAlreadyExistsExceptionTests
{
    [Fact]
    public void Constructor_Default_SetsEmptyAlias()
    {
        // Act
        var exception = new AliasAlreadyExistsException();

        // Assert
        Assert.Equal(string.Empty, exception.Alias);
    }

    [Fact]
    public void Constructor_WithAlias_SetsProperties()
    {
        // Arrange
        const string alias = "v";

        // Act
        var exception = new AliasAlreadyExistsException(alias);

        // Assert
        Assert.Equal(alias, exception.Alias);
        Assert.Equal($"An alias '{alias}' is already registered for another command.", exception.Message);
    }

    [Fact]
    public void Constructor_WithAliasAndInnerException_SetsProperties()
    {
        // Arrange
        const string alias = "h";
        var inner = new Exception("Collision detected");

        // Act
        var exception = new AliasAlreadyExistsException(alias, inner);

        // Assert
        Assert.Equal(alias, exception.Alias);
        Assert.Same(inner, exception.InnerException);
        Assert.Equal($"An alias '{alias}' is already registered for another command.", exception.Message);
    }

    [Fact]
    public void Serialization_Constructor_RestoresAlias()
    {
        // Arrange
        const string alias = "duplicate-alias";
        var info = new SerializationInfo(typeof(AliasAlreadyExistsException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        // Required base Exception fields to prevent SerializationException during base(info, context)
        info.AddValue("ClassName", typeof(AliasAlreadyExistsException).FullName);
        info.AddValue("Message", "Custom message");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // The custom field for this exception
        info.AddValue("Alias", alias);

        // Act - Invoke protected constructor
        var constructor = typeof(AliasAlreadyExistsException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(SerializationInfo), typeof(StreamingContext) },
            null);

        var exception = (AliasAlreadyExistsException)constructor!.Invoke(new object[] { info, context });

        // Assert
        Assert.Equal(alias, exception.Alias);
    }

    [Fact]
    public void Serialization_Constructor_WithNullAlias_SetsEmptyString()
    {
        // Arrange
        var info = new SerializationInfo(typeof(AliasAlreadyExistsException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        info.AddValue("ClassName", typeof(AliasAlreadyExistsException).FullName);
        info.AddValue("Message", "Error");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // Simulate a null value for Alias in the serialization bag
        info.AddValue("Alias", null);

        // Act
        var constructor = typeof(AliasAlreadyExistsException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(SerializationInfo), typeof(StreamingContext) },
            null);

        var exception = (AliasAlreadyExistsException)constructor!.Invoke(new object[] { info, context });

        // Assert
        Assert.Equal(string.Empty, exception.Alias);
    }
}