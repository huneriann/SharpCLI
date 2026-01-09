namespace SharpCLI.Tests.UnitTests.ExceptionsUnitTests;

public class CommandAlreadyExistsExceptionTests
{
    [Fact]
    public void Constructor_Default_SetsEmptyCommandName()
    {
        // Act
        var exception = new CommandAlreadyExistsException();

        // Assert
        Assert.Equal(string.Empty, exception.CommandName);
    }

    [Fact]
    public void Constructor_WithCommandName_SetsProperties()
    {
        // Arrange
        const string commandName = "build";

        // Act
        var exception = new CommandAlreadyExistsException(commandName);

        // Assert
        Assert.Equal(commandName, exception.CommandName);
        Assert.Equal($"A command with the name '{commandName}' is already registered.", exception.Message);
    }

    [Fact]
    public void Constructor_WithCommandNameAndInnerException_SetsProperties()
    {
        // Arrange
        const string commandName = "deploy";
        var inner = new Exception("Internal collision");

        // Act
        var exception = new CommandAlreadyExistsException(commandName, inner);

        // Assert
        Assert.Equal(commandName, exception.CommandName);
        Assert.Same(inner, exception.InnerException);
        Assert.Equal($"A command with the name '{commandName}' is already registered.", exception.Message);
    }

    [Fact]
    public void Serialization_Constructor_RestoresCommandName()
    {
        // Arrange
        const string commandName = "test-cmd";
        var info = new SerializationInfo(typeof(CommandAlreadyExistsException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        // Required base Exception fields for base(info, context)
        info.AddValue("ClassName", typeof(CommandAlreadyExistsException).FullName);
        info.AddValue("Message", "Custom message");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // Custom field specific to this exception
        info.AddValue("CommandName", commandName);

        // Act - Invoke protected constructor via reflection
        var constructor = typeof(CommandAlreadyExistsException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (CommandAlreadyExistsException)constructor!.Invoke([info, context]);

        // Assert
        Assert.Equal(commandName, exception.CommandName);
    }

    [Fact]
    public void Serialization_Constructor_WithNullCommandName_SetsEmptyString()
    {
        // Arrange
        var info = new SerializationInfo(typeof(CommandAlreadyExistsException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        info.AddValue("ClassName", typeof(CommandAlreadyExistsException).FullName);
        info.AddValue("Message", "Error");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // Simulate null entry for CommandName in the serialization bag
        info.AddValue("CommandName", null);

        // Act
        var constructor = typeof(CommandAlreadyExistsException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (CommandAlreadyExistsException)constructor!.Invoke([info, context]);

        // Assert
        Assert.Equal(string.Empty, exception.CommandName);
    }
}