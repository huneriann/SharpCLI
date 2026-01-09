namespace SharpCLI.Tests.UnitTests.ExceptionsUnitTests;

public class MissingRequiredArgumentExceptionTests
{
    [Fact]
    public void Constructor_Default_SetsEmptyArgumentName()
    {
        // Act
        var exception = new MissingRequiredArgumentException();

        // Assert
        Assert.Equal(string.Empty, exception.ArgumentName);
    }

    [Fact]
    public void Constructor_WithArgumentName_SetsProperties()
    {
        // Arrange
        const string argName = "filePath";

        // Act
        var exception = new MissingRequiredArgumentException(argName);

        // Assert
        Assert.Equal(argName, exception.ArgumentName);
        Assert.Equal($"Required argument '{argName}' is missing.", exception.Message);
    }

    [Fact]
    public void Constructor_WithArgumentNameAndInnerException_SetsProperties()
    {
        // Arrange
        const string argName = "input";
        var inner = new Exception("Root cause");

        // Act
        var exception = new MissingRequiredArgumentException(argName, inner);

        // Assert
        Assert.Equal(argName, exception.ArgumentName);
        Assert.Same(inner, exception.InnerException);
        Assert.Equal($"Required argument '{argName}' is missing.", exception.Message);
    }

    [Fact]
    public void Serialization_Constructor_RestoresArgumentName()
    {
        // Arrange
        const string argName = "config-path";
        var info = new SerializationInfo(typeof(MissingRequiredArgumentException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        // Required base Exception fields for base(info, context)
        info.AddValue("ClassName", typeof(MissingRequiredArgumentException).FullName);
        info.AddValue("Message", "Custom message");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // Custom field
        info.AddValue("ArgumentName", argName);

        // Act - Invoke protected constructor via reflection
        var constructor = typeof(MissingRequiredArgumentException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (MissingRequiredArgumentException)constructor!.Invoke([info, context]);

        // Assert
        Assert.Equal(argName, exception.ArgumentName);
    }

    [Fact]
    public void Serialization_Constructor_WithNullArgumentName_SetsEmptyString()
    {
        // Arrange
        var info = new SerializationInfo(typeof(MissingRequiredArgumentException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        info.AddValue("ClassName", typeof(MissingRequiredArgumentException).FullName);
        info.AddValue("Message", "Error");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // Simulate null entry for ArgumentName
        info.AddValue("ArgumentName", null);

        // Act
        var constructor = typeof(MissingRequiredArgumentException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (MissingRequiredArgumentException)constructor!.Invoke([info, context]);

        // Assert
        Assert.Equal(string.Empty, exception.ArgumentName);
    }
}