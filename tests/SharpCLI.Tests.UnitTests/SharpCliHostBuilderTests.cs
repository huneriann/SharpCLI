namespace SharpCLI.Tests.UnitTests;

public class SharpCliHostBuilderTests
{
    [Fact]
    public void Build_WithNameSet_CreatesHostWithCorrectName()
    {
        // Arrange
        var builder = new SharpCliHostBuilder();

        // Act
        var host = builder.Name("MyApp").Build();

        // Assert
        // Access via reflection since fields are private
        var nameField = typeof(SharpCliHost).GetField("_name", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var nameValue = (string?)nameField?.GetValue(host);

        Assert.Equal("MyApp", nameValue);
    }

    [Fact]
    public void Build_WithAllPropertiesSet_ConfiguresHostCorrectly()
    {
        // Arrange
        var builder = new SharpCliHostBuilder();
        var customWriter = new StringWriter();
        const string customMessage = "Welcome to MyApp!\nUse --help for usage.";

        // Act
        var host = builder
            .Name("CoolTool")
            .Description("A very cool command-line tool")
            .Writer(customWriter)
            .CustomHelpMessage(customMessage)
            .Build();

        // Assert
        var nameField = typeof(SharpCliHost).GetField("_name", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var descField = typeof(SharpCliHost).GetField("_description", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var writerField = typeof(SharpCliHost).GetField("_writer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var customMsgField = typeof(SharpCliHost).GetField("_customHelpMessage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.Equal("CoolTool", nameField?.GetValue(host));
        Assert.Equal("A very cool command-line tool", descField?.GetValue(host));
        Assert.Same(customWriter, writerField?.GetValue(host));
        Assert.Equal(customMessage, customMsgField?.GetValue(host));
    }

    [Fact]
    public void Build_WithoutName_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new SharpCliHostBuilder();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Equal("Application name must be set using .Name()", ex.Message);
    }

    [Fact]
    public void Build_WithNullName_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new SharpCliHostBuilder();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => builder.Name(null!).Build());
        Assert.Equal("Application name must be set using .Name()", ex.Message);
    }

    [Fact]
    public void Build_WithWhitespaceName_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new SharpCliHostBuilder();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => builder.Name("   ").Build());
        Assert.Equal("Application name must be set using .Name()", ex.Message);
    }

    [Fact]
    public void Description_WithNull_SetsEmptyString()
    {
        // Arrange
        var builder = new SharpCliHostBuilder();

        // Act
        var host = builder.Name("App").Description(string.Empty).Build();

        // Assert
        var descField = typeof(SharpCliHost).GetField("_description", 
            BindingFlags.NonPublic | 
            BindingFlags.Instance);
        
        Assert.Equal(string.Empty, descField?.GetValue(host));
    }

    [Fact]
    public void Writer_WithNull_UsesConsoleOutByDefault()
    {
        // Arrange
        var builder = new SharpCliHostBuilder();

        // Act
        var host = builder.Name("App").Writer(null).Build();

        // Assert
        var writerField = typeof(SharpCliHost).GetField("_writer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.Same(Console.Out, writerField?.GetValue(host));
    }

    [Fact]
    public void CustomHelpMessage_WithNull_SetsNull()
    {
        // Arrange
        var builder = new SharpCliHostBuilder();

        // Act
        var host = builder.Name("App").CustomHelpMessage(null).Build();

        // Assert
        var customMsgField = typeof(SharpCliHost).GetField("_customHelpMessage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.Null(customMsgField?.GetValue(host));
    }

    [Fact]
    public void CustomHelpMessage_WithEmptyString_SetsEmptyString()
    {
        // Arrange
        var builder = new SharpCliHostBuilder();

        // Act
        var host = builder.Name("App").CustomHelpMessage("").Build();

        // Assert
        var customMsgField = typeof(SharpCliHost).GetField("_customHelpMessage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.Equal("", customMsgField?.GetValue(host));
    }

    [Fact]
    public void FluentChaining_AllMethodsReturnSameBuilderInstance()
    {
        // Arrange
        var builder = new SharpCliHostBuilder();
        var customWriter = new StringWriter();

        // Act
        var result = builder
            .Name("FluentApp")
            .Description("Testing fluency")
            .Writer(customWriter)
            .CustomHelpMessage("Hello!");

        // Assert
        Assert.Same(builder, result);
    }

    [Fact]
    public void Build_MinimalValidConfiguration_Works()
    {
        // Arrange
        var builder = new SharpCliHostBuilder();

        // Act
        var host = builder.Name("Minimal").Build();

        // Assert
        var nameField = typeof(SharpCliHost).GetField("_name", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var descField = typeof(SharpCliHost).GetField("_description", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var writerField = typeof(SharpCliHost).GetField("_writer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var customMsgField = typeof(SharpCliHost).GetField("_customHelpMessage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.Equal("Minimal", nameField?.GetValue(host));
        Assert.Equal("", descField?.GetValue(host));
        Assert.Same(Console.Out, writerField?.GetValue(host));
        Assert.Null(customMsgField?.GetValue(host));
    }
}