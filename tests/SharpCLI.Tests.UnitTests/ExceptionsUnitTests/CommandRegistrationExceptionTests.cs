namespace SharpCLI.Tests.UnitTests.ExceptionsUnitTests;

public class CommandRegistrationExceptionTests
{
    [Fact]
    public void Constructor_Default_InitializesCorrectly()
    {
        // Act
        var exception = new CommandRegistrationException();

        // Assert
        Assert.NotNull(exception);
        Assert.Contains(nameof(CommandRegistrationException), exception.Message);
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        const string message = "Failed to register command 'test'.";

        // Act
        var exception = new CommandRegistrationException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        // Arrange
        const string message = "Registration error.";
        var inner = new Exception("Constraint violation.");

        // Act
        var exception = new CommandRegistrationException(message, inner);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void Serialization_Constructor_InitializesViaReflection()
    {
        // Arrange
        var info = new SerializationInfo(typeof(CommandRegistrationException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        // Standard Exception fields required for successful base(info, context) call
        info.AddValue("ClassName", typeof(CommandRegistrationException).FullName);
        info.AddValue("Message", "Serialized registration error");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // Act - Access the protected constructor
        var constructor = typeof(CommandRegistrationException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (CommandRegistrationException)constructor!.Invoke([info, context]);

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("Serialized registration error", exception.Message);
    }
}