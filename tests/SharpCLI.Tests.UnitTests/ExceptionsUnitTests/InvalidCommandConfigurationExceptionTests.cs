namespace SharpCLI.Tests.UnitTests.ExceptionsUnitTests;

public class InvalidCommandConfigurationExceptionTests
{
    [Fact]
    public void Constructor_Default_InitializesCorrectly()
    {
        // Act
        var exception = new InvalidCommandConfigurationException();

        // Assert
        Assert.NotNull(exception);
        Assert.Contains(nameof(InvalidCommandConfigurationException), exception.Message);
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        const string message = "Command 'run' is missing a required handler.";

        // Act
        var exception = new InvalidCommandConfigurationException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        // Arrange
        const string message = "Configuration error.";
        var inner = new InvalidOperationException("Dependency not met.");

        // Act
        var exception = new InvalidCommandConfigurationException(message, inner);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void Serialization_Constructor_InitializesViaReflection()
    {
        // Arrange
        var info = new SerializationInfo(typeof(InvalidCommandConfigurationException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        // Mandatory fields required by System.Exception's serialization constructor
        info.AddValue("ClassName", typeof(InvalidCommandConfigurationException).FullName);
        info.AddValue("Message", "Serialized config error");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // Act - Invoke the protected serialization constructor
        var constructor = typeof(InvalidCommandConfigurationException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (InvalidCommandConfigurationException)constructor!.Invoke([info, context]);

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("Serialized config error", exception.Message);
    }
}