namespace SharpCLI.Tests.UnitTests.ExceptionsUnitTests;

public class ArgumentParsingExceptionTests
{
    [Fact]
    public void Constructor_Default_InitializesCorrectly()
    {
        // Act
        var exception = new ArgumentParsingException();

        // Assert
        Assert.NotNull(exception);
        
        // Default Exception message usually contains the class name
        Assert.Contains(nameof(ArgumentParsingException), exception.Message);
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        const string message = "Invalid argument provided.";

        // Act
        var exception = new ArgumentParsingException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        // Arrange
        const string message = "Parsing failed.";
        var inner = new FormatException("Invalid format.");

        // Act
        var exception = new ArgumentParsingException(message, inner);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void Serialization_Constructor_InitializesViaReflection()
    {
        // Arrange
        var info = new SerializationInfo(typeof(ArgumentParsingException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        // Mandatory fields required by System.Exception's serialization constructor
        info.AddValue("ClassName", typeof(ArgumentParsingException).FullName);
        info.AddValue("Message", "Serialized message");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // Act - Invoke the protected serialization constructor
        var constructor = typeof(ArgumentParsingException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (ArgumentParsingException)constructor!.Invoke([info, context]);

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("Serialized message", exception.Message);
    }
}