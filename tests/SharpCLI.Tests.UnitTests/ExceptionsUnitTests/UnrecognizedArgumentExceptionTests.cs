namespace SharpCLI.Tests.UnitTests.ExceptionsUnitTests;

public class UnrecognizedArgumentExceptionTests
{
    [Fact]
    public void Constructor_Default_SetsEmptyUnrecognizedArg()
    {
        // Act
        var exception = new UnrecognizedArgumentException();

        // Assert
        Assert.Equal(string.Empty, exception.UnrecognizedArg);
    }

    [Fact]
    public void Constructor_WithUnrecognizedArg_SetsProperties()
    {
        // Arrange
        const string arg = "--unknown-flag";

        // Act
        var exception = new UnrecognizedArgumentException(arg);

        // Assert
        Assert.Equal(arg, exception.UnrecognizedArg);
        Assert.Equal($"Unrecognized argument or option: '{arg}'.", exception.Message);
    }

    [Fact]
    public void Constructor_WithUnrecognizedArgAndInnerException_SetsProperties()
    {
        // Arrange
        const string arg = "invalid-param";
        var inner = new Exception("Validation failed");

        // Act
        var exception = new UnrecognizedArgumentException(arg, inner);

        // Assert
        Assert.Equal(arg, exception.UnrecognizedArg);
        Assert.Same(inner, exception.InnerException);
        Assert.Equal($"Unrecognized argument or option: '{arg}'.", exception.Message);
    }

    [Fact]
    public void Serialization_Constructor_RestoresUnrecognizedArg()
    {
        // Arrange
        const string arg = "unexpected-token";
        var info = new SerializationInfo(typeof(UnrecognizedArgumentException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        // Mandatory fields required by System.Exception's serialization constructor
        info.AddValue("ClassName", typeof(UnrecognizedArgumentException).FullName);
        info.AddValue("Message", "Custom error message");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // The property-specific data
        info.AddValue("UnrecognizedArg", arg);

        // Act - Invoke the protected constructor via reflection
        var constructor = typeof(UnrecognizedArgumentException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (UnrecognizedArgumentException)constructor!.Invoke([info, context]);

        // Assert
        Assert.Equal(arg, exception.UnrecognizedArg);
    }

    [Fact]
    public void Serialization_Constructor_WithNullUnrecognizedArg_SetsEmptyString()
    {
        // Arrange
        var info = new SerializationInfo(typeof(UnrecognizedArgumentException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        info.AddValue("ClassName", typeof(UnrecognizedArgumentException).FullName);
        info.AddValue("Message", "Error");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // Simulate a null entry in the serialization data to test the '?? string.Empty' logic
        info.AddValue("UnrecognizedArg", null);

        // Act
        var constructor = typeof(UnrecognizedArgumentException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (UnrecognizedArgumentException)constructor!.Invoke([info, context]);

        // Assert
        Assert.Equal(string.Empty, exception.UnrecognizedArg);
    }
}