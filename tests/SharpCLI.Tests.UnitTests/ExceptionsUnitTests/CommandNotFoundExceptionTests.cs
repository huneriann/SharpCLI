namespace SharpCLI.Tests.UnitTests.ExceptionsUnitTests;

public class CommandNotFoundExceptionTests
{
    [Fact]
    public void Constructor_Default_SetsEmptyCommandName()
    {
        // Act
        var exception = new CommandNotFoundException();

        // Assert
        Assert.Equal(string.Empty, exception.CommandName);
    }

    [Fact]
    public void Constructor_WithCommandName_SetsProperties()
    {
        // Arrange
        const string commandName = "unknown";

        // Act
        var exception = new CommandNotFoundException(commandName);

        // Assert
        Assert.Equal(commandName, exception.CommandName);
        Assert.Equal($"Command '{commandName}' not found. Use '--help' for available commands.", exception.Message);
    }

    [Fact]
    public void Constructor_WithCommandNameAndInnerException_SetsProperties()
    {
        // Arrange
        const string commandName = "missing-tool";
        var inner = new Exception("Source system failure");

        // Act
        var exception = new CommandNotFoundException(commandName, inner);

        // Assert
        Assert.Equal(commandName, exception.CommandName);
        Assert.Same(inner, exception.InnerException);
        Assert.Equal($"Command '{commandName}' not found. Use '--help' for available commands.", exception.Message);
    }

    [Fact]
    public void Serialization_Constructor_RestoresCommandName()
    {
        // Arrange
        const string commandName = "ghost-command";
        var info = new SerializationInfo(typeof(CommandNotFoundException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        // Required base Exception fields for base(info, context)
        info.AddValue("ClassName", typeof(CommandNotFoundException).FullName);
        info.AddValue("Message", "Custom message");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // Custom field for CommandNotFoundException
        info.AddValue("CommandName", commandName);

        // Act - Invoke protected constructor via reflection
        var constructor = typeof(CommandNotFoundException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (CommandNotFoundException)constructor!.Invoke([info, context]);

        // Assert
        Assert.Equal(commandName, exception.CommandName);
    }

    [Fact]
    public void Serialization_Constructor_WithNullCommandName_SetsEmptyString()
    {
        // Arrange
        var info = new SerializationInfo(typeof(CommandNotFoundException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        info.AddValue("ClassName", typeof(CommandNotFoundException).FullName);
        info.AddValue("Message", "Error");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("HResult", -2146233088);
        info.AddValue("Source", null);

        // Simulate a null entry to test the null-coalescing logic
        info.AddValue("CommandName", null);

        // Act
        var constructor = typeof(CommandNotFoundException).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            null);

        var exception = (CommandNotFoundException)constructor!.Invoke([info, context]);

        // Assert
        Assert.Equal(string.Empty, exception.CommandName);
    }
}