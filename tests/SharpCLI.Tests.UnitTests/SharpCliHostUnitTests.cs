namespace SharpCLI.Tests.UnitTests;

public class SharpCliHostUnitTests
{
    private readonly SharpCliHost _host;
    private readonly TestCommands _mockCommands;

    public SharpCliHostUnitTests()
    {
        _host = new SharpCliHost("TestApp", "A test CLI");
        _mockCommands = new TestCommands();
        _host.RegisterCommands(_mockCommands);
    }

    [Fact]
    public async Task RunAsync_ValidCommand_ExecutesSuccessfully()
    {
        // Arrange
        string[] args = { "test", "hello", "-c", "5" };

        // Act
        var result = await _host.RunAsync(args);

        // Assert
        Assert.Equal(0, result);
        Assert.True(_mockCommands.WasExecuted);
        Assert.Equal("hello-5", _mockCommands.LastValue);
    }

    [Fact]
    public async Task RunAsync_Alias_ExecutesSuccessfully()
    {
        // Arrange
        string[] args = { "t", "alias-test" }; // 't' is alias for 'test'

        // Act
        var result = await _host.RunAsync(args);

        // Assert
        Assert.Equal(0, result);
        Assert.True(_mockCommands.WasExecuted);
    }

    [Fact]
    public async Task RunAsync_BooleanFlag_ParsesCorrectly()
    {
        // Arrange
        string[] argsPresent = { "bool-test", "--force" };
        string[] argsMissing = { "bool-test" };

        // Act
        var resultPresent = await _host.RunAsync(argsPresent);
        var resultMissing = await _host.RunAsync(argsMissing);

        // Assert
        Assert.Equal(1, resultPresent);
        Assert.Equal(0, resultMissing);
    }

    [Fact]
    public async Task RunAsync_AsyncMethod_ReturnsExpectedValue()
    {
        // Arrange
        string[] args = { "async-test" };

        // Act
        var result = await _host.RunAsync(args);

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task RunAsync_UnknownCommand_ReturnsErrorCode()
    {
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);

        // Arrange
        string[] args = { "ghost-command" };

        // Act
        var result = await host.RunAsync(args);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task RunAsync_MissingRequiredArgument_HandlesError()
    {
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);

        // Arrange
        string[] args = { "test" };

        // Act
        var result = await host.RunAsync(args);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task RunAsync_HelpArgument_ReturnsZero()
    {
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);

        // Arrange
        string[] args = { "--help" };

        // Act
        var result = await host.RunAsync(args);

        // Assert
        Assert.Equal(0, result);
        Assert.Contains("USAGE", sw.ToString());
    }

    [Fact]
    public async Task RunAsync_ComplexTypes_ParsesCorrectly()
    {
        // Arrange
        string[] args = { "types", "42", "3.14", "true", "High" };

        // Act
        await _host.RunAsync(args);

        // Assert
        Assert.Equal("42|3.14|True|High", _mockCommands.LastValue);
    }

    [Fact]
    public async Task RunAsync_OptionMissingValue_ReturnsErrorCode()
    {
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);
        host.RegisterCommands(_mockCommands);

        // Act:
        var result = await host.RunAsync(["test", "val", "--count"]);

        // Assert
        Assert.Equal(1, result);
        Assert.Contains("Option count requires a value", sw.ToString());
    }

    [Fact]
    public async Task RunAsync_MultipleArguments_RespectsOrder()
    {
        // Arrange
        string[] args = { "multi-arg", "A", "B" };

        // Act
        await _host.RunAsync(args);

        // Assert
        Assert.Equal("A-B", _mockCommands.LastValue);
    }
}