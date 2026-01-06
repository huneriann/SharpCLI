namespace SharpCLI.Tests.UnitTests;

public class SharpCliHostUnitTests
{
    private readonly SharpCliHost _host;
    private readonly BasicCommands _mockCommands;

    public SharpCliHostUnitTests()
    {
        _host = new SharpCliHost("TestApp", "A test CLI");
        _mockCommands = new BasicCommands();
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
        var ex = await Assert.ThrowsAsync<CommandNotFoundException>(() => host.RunAsync(args));

        // Assert
        Assert.Equal("ghost-command", ex.CommandName);
    }

    [Fact]
    public async Task RunAsync_MissingRequiredArgument_HandlesError()
    {
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);
        host.RegisterCommands(new BasicCommands());

        // Arrange
        string[] args = { "test" };

        // Act
        var result = await host.RunAsync(args);

        // Assert
        Assert.Equal(1, result);
        Assert.Contains("Required argument", sw.ToString());
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
        host.RegisterCommands(new BasicCommands());

        // Arrange
        string[] args = { "test", "val", "--count" };

        // Act
        var result = await host.RunAsync(args);

        // Assert
        Assert.Equal(1, result);
        Assert.Contains("Option 'count' requires a value but none was provided.", sw.ToString());
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

    // New tests below

    [Fact]
    public void RegisterCommands_DuplicateCommand_ThrowsCommandAlreadyExistsException()
    {
        // Act & Assert
        var ex = Assert.Throws<CommandAlreadyExistsException>(() => _host.RegisterCommands(_mockCommands));
        Assert.Equal("test", ex.CommandName); // Assuming "test" is a command name
    }

    [Fact]
    public void RegisterCommands_DuplicateAlias_ThrowsAliasAlreadyExistsException()
    {
        // Arrange
        var conflictingCommands = new ConflictingAliasCommands(); // Assume this has command with alias "t"

        // Act & Assert
        var ex = Assert.Throws<AliasAlreadyExistsException>(() => _host.RegisterCommands(conflictingCommands));
        Assert.Equal("t", ex.Alias);
    }

    [Fact]
    public void RegisterCommands_InvalidReturnType_ThrowsInvalidCommandConfigurationException()
    {
        // Arrange
        var badCommands = new BadReturnTypeCommands(); // Assume method with invalid return like string

        var host = new SharpCliHost("TestApp", "Desc");

        // Act & Assert
        var ex =
            Assert.Throws<InvalidCommandConfigurationException>(() => host.RegisterCommands(badCommands));
        Assert.Contains("Unsupported return type", ex.Message);
    }

    [Fact]
    public void RegisterCommands_DuplicateParameterNames_ThrowsInvalidCommandConfigurationException()
    {
        // Arrange
        var duplicateParamCommands =
            new DuplicateParamNamesCommands(); // Assume method with [Argument(Name="same")] on two params

        var host = new SharpCliHost("TestApp", "Desc");

        // Act & Assert
        var ex = Assert.Throws<InvalidCommandConfigurationException>(() =>
            host.RegisterCommands(duplicateParamCommands));
        Assert.Contains("Duplicate parameter names found", ex.Message);
    }

    [Fact]
    public async Task RunAsync_InvalidArgumentValue_HandlesError()
    {
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);
        host.RegisterCommands(new BasicCommands());

        // Arrange
        string[] args = { "test", "hello", "-c", "notAnInt" };

        // Act
        var result = await host.RunAsync(args);

        // Assert
        Assert.Equal(1, result);
        Assert.Contains("Invalid value 'notAnInt' for 'count'. Expected type: Int32.", sw.ToString());
    }

    [Fact]
    public async Task RunAsync_UnrecognizedOption_HandlesError()
    {
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);
        host.RegisterCommands(new BasicCommands());

        // Arrange
        string[] args = { "test", "hello", "--unknown" };

        // Act
        var result = await host.RunAsync(args);

        // Assert
        Assert.Equal(1, result);
        Assert.Contains("Unrecognized argument or option: '--unknown'", sw.ToString());
    }

    [Fact]
    public async Task RunAsync_ExtraPositionalArguments_HandlesError()
    {
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);
        host.RegisterCommands(new BasicCommands());

        // Arrange
        string[] args = { "test", "hello", "-c", "5", "extra1", "extra2" };

        // Act
        var result = await host.RunAsync(args);

        // Assert
        Assert.Equal(1, result);
        Assert.Contains("Unrecognized argument or option: 'extra1 extra2'", sw.ToString());
    }

    [Fact]
    public async Task RunAsync_InvalidEnumValue_HandlesError()
    {
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);
        host.RegisterCommands(new BasicCommands());

        // Arrange
        string[] args = { "types", "42", "3.14", "true", "InvalidEnum" };

        // Act
        var result = await host.RunAsync(args);

        // Assert
        Assert.Equal(1, result);
        Assert.Contains("Invalid value 'InvalidEnum' for", sw.ToString());
    }

    [Fact]
    public async Task RunAsync_CommandSpecificHelp_ShowsHelp()
    {
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);
        host.RegisterCommands(new BasicCommands());

        // Arrange
        string[] args = { "test", "--help" };

        // Act
        var result = await host.RunAsync(args);

        // Assert
        Assert.Equal(0, result);
        Assert.Contains("Usage: TestApp test [OPTIONS] [ARGUMENTS]", sw.ToString());
    }
}