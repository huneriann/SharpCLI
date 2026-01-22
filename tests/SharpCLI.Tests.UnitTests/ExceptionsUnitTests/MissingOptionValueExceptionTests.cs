namespace SharpCLI.Tests.UnitTests.ExceptionsUnitTests;

public class MissingOptionValueExceptionTests
{
    [Fact]
    public void Constructor_Default_SetsEmptyOptionName()
    {
        // Act
        var exception = new MissingOptionValueException();

        // Assert
        Assert.Equal(string.Empty, exception.OptionName);
    }

    [Fact]
    public void Constructor_WithOptionName_SetsProperties()
    {
        // Arrange
        const string optionName = "threshold";

        // Act
        var exception = new MissingOptionValueException(optionName);

        // Assert
        Assert.Equal(optionName, exception.OptionName);
        Assert.Equal($"Option '{optionName}' requires a value but none was provided.", exception.Message);
    }

    [Fact]
    public void Constructor_WithOptionNameAndInnerException_SetsProperties()
    {
        // Arrange
        const string optionName = "output";
        var inner = new Exception("Root cause");

        // Act
        var exception = new MissingOptionValueException(optionName, inner);

        // Assert
        Assert.Equal(optionName, exception.OptionName);
        Assert.Same(inner, exception.InnerException);
        Assert.Equal($"Option '{optionName}' requires a value but none was provided.", exception.Message);
    }

    [Fact]
    public void SerializationConstructor_ValidInfo_SetsOptionName()
    {
        // Arrange
        var info = new SerializationInfo(typeof(MissingOptionValueException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        // We must provide the fields that the 'base(info, context)' call expects
        // to avoid "Member 'Message' not found" errors.
        info.AddValue("ClassName", typeof(MissingOptionValueException).FullName);
        info.AddValue("Message", "Custom error message");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088); // Standard Exception HResult
        info.AddValue("Source", null);

        // This is the specific line your constructor logic depends on:
        info.AddValue("OptionName", "test-option");

        // Act - Invoke the protected constructor via reflection
        var constructor = typeof(MissingOptionValueException).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (MissingOptionValueException)constructor!.Invoke([info, context]);

        // Assert
        Assert.Equal("test-option", exception.OptionName);
    }

    [Fact]
    public void SerializationConstructor_NullOptionName_SetsEmptyString()
    {
        // Arrange
        var info = new SerializationInfo(typeof(MissingOptionValueException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        // Populate mandatory base exception fields
        info.AddValue("ClassName", typeof(MissingOptionValueException).FullName);
        info.AddValue("Message", "Error");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // Simulate a null value for OptionName to test the '?? string.Empty' logic
        info.AddValue("OptionName", null);

        // Act
        var constructor = typeof(MissingOptionValueException).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (MissingOptionValueException)constructor!.Invoke([info, context]);

        // Assert
        Assert.Equal(string.Empty, exception.OptionName);
    }
}