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
        string[] args = ["test", "hello", "-c", "5"];

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
        string[] args = ["t", "alias-test"]; // 't' is alias for 'test'

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
        string[] argsPresent = ["bool-test", "--force"];
        string[] argsMissing = ["bool-test"];

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
        string[] args = ["async-test"];

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
        string[] args = ["ghost-command"];

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
        string[] args = ["test"];

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
        string[] args = ["--help"];

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
        string[] args = ["types", "42", "3.14", "true", "High"];

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
        string[] args = ["test", "val", "--count"];

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
        string[] args = ["multi-arg", "A", "B"];

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
        
        Assert.Equal("test", ex.CommandName);
    }

    [Fact]
    public void RegisterCommands_DuplicateAlias_ThrowsAliasAlreadyExistsException()
    {
        // Arrange
        var conflictingCommands = new ConflictingAliasCommands();

        // Act & Assert
        var ex = Assert.Throws<AliasAlreadyExistsException>(() => _host.RegisterCommands(conflictingCommands));
        Assert.Equal("t", ex.Alias);
    }

    [Fact]
    public void RegisterCommands_InvalidReturnType_ThrowsInvalidCommandConfigurationException()
    {
        // Arrange
        var badCommands = new BadReturnTypeCommands();

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
            new DuplicateParamNamesCommands();

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
        string[] args = ["test", "hello", "-c", "notAnInt"];

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
        string[] args = ["test", "hello", "--unknown"];

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
        string[] args = ["test", "hello", "-c", "5", "extra1", "extra2"];

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
        string[] args = ["types", "42", "3.14", "true", "InvalidEnum"];

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
        string[] args = ["test", "--help"];

        // Act
        var result = await host.RunAsync(args);

        // Assert
        Assert.Equal(0, result);
        Assert.Contains("Usage: TestApp test [OPTIONS] [ARGUMENTS]", sw.ToString());
    }

    [Fact]
    public void Dispose_WithCustomWriter_DisposesWriter()
    {
        // Arrange
        var mockWriter = new DisposeTrackerWriter();
        var host = new SharpCliHost("TestApp", "Description", mockWriter);

        // Act
        host.Dispose();

        // Assert
        Assert.True(mockWriter.IsDisposed, "The provided TextWriter was not disposed.");
    }

    [Fact]
    public void Dispose_WithDefaultWriter_DoesNotThrow()
    {
        // Arrange
        var host = new SharpCliHost("TestApp", "Description", null);

        // Act
        var exception = Record.Exception(() => host.Dispose());

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task RegisterCommandsGeneric_StaticMethods_RegistersSuccessfully()
    {
        // Arrange
        var host = new SharpCliHost("TestApp");

        // Act
        host.RegisterCommands<StaticCommands>();

        // Assert
        string[] args = ["static-test"];
        var result = await host.RunAsync(args);

        Assert.Equal(0, result);
        Assert.True(StaticCommands.WasExecuted);
    }

    [Fact]
    public async Task RegisterCommandsGeneric_InstanceMethods_AreIgnored()
    {
        // Arrange
        var host = new SharpCliHost("TestApp");

        // Act
        host.RegisterCommands<MixedCommands>();

        // Assert
        Assert.NotNull(host);

        string[] args = ["instance-test"];
        await Assert.ThrowsAsync<CommandNotFoundException>(() => host.RunAsync(args));
    }

    [Fact]
    public void RegisterCommandsGeneric_ReturnsHostInstance()
    {
        // Arrange
        var host = new SharpCliHost("TestApp");

        // Act
        var returnedHost = host.RegisterCommands<StaticCommands>();

        // Assert
        Assert.Same(host, returnedHost);
    }

    [Fact]
    public void ParameterlessConstructor_SetsDefaultProperties()
    {
        // Act
        var ex = new InvalidArgumentValueException();

        // Assert
        Assert.Empty(ex.ArgumentName);
        Assert.Empty(ex.ProvidedValue);
        Assert.Equal(typeof(object), ex.ExpectedType);
    }

    [Fact]
    public void MessageConstructor_SetsMessageAndDefaults()
    {
        // Arrange
        var message = "Custom error message";

        // Act
        var ex = new InvalidArgumentValueException(message);

        // Assert
        Assert.Equal(message, ex.Message);
        Assert.Empty(ex.ArgumentName);
        Assert.Empty(ex.ProvidedValue);
        Assert.Equal(typeof(object), ex.ExpectedType);
    }

    [Fact]
    public void MessageAndInnerExceptionConstructor_SetsProperties()
    {
        // Arrange
        var message = "Error message";
        var inner = new Exception("Inner");

        // Act
        var ex = new InvalidArgumentValueException(message, inner);

        // Assert
        Assert.Equal(message, ex.Message);
        Assert.Same(inner, ex.InnerException);
        Assert.Empty(ex.ArgumentName);
    }

    [Fact]
    public void DetailedConstructor_SetsPropertiesAndFormattedMessage()
    {
        // Arrange
        var argName = "count";
        var val = "abc";
        var type = typeof(int);
        var inner = new Exception("Parsing failed");

        // Act
        var ex = new InvalidArgumentValueException(argName, val, type, inner);

        // Assert
        Assert.Equal(argName, ex.ArgumentName);
        Assert.Equal(val, ex.ProvidedValue);
        Assert.Equal(type, ex.ExpectedType);
        Assert.Same(inner, ex.InnerException);
        Assert.Contains("'abc'", ex.Message);
        Assert.Contains("'count'", ex.Message);
        Assert.Contains("Int32", ex.Message);
    }

    [Fact]
    public void SerializationConstructor_RoundTripsCorrectly()
    {
        // Arrange
        var originalEx = new InvalidArgumentValueException("limit", "high", typeof(double));

        var info = new SerializationInfo(typeof(InvalidArgumentValueException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        // Manually populate the info to simulate the serialization process
        info.AddValue("ArgumentName", "limit");
        info.AddValue("ProvidedValue", "high");
        info.AddValue("ExpectedType", typeof(double));
        // Base Exception fields
        info.AddValue("ClassName", "InvalidArgumentValueException");
        info.AddValue("Message", originalEx.Message);
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("ExceptionMethod", null);
        info.AddValue("HResult", originalEx.HResult);
        info.AddValue("Source", null);

        // Act
        var constructor = typeof(InvalidArgumentValueException).GetConstructor(
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            null, [typeof(SerializationInfo), typeof(StreamingContext)], null);

        var deserializedEx = (InvalidArgumentValueException)constructor!.Invoke([info, context]);

        // Assert
        Assert.Equal("limit", deserializedEx.ArgumentName);
        Assert.Equal("high", deserializedEx.ProvidedValue);
        Assert.Equal(typeof(double), deserializedEx.ExpectedType);
    }

    [Fact]
    public void AllConstructors_And_Properties_Coverage()
    {
        // 1. Parameterless
        var ex1 = new InvalidArgumentValueException();
        Assert.Empty(ex1.ArgumentName);

        // 2. Message only
        var ex2 = new InvalidArgumentValueException("Error");
        Assert.Equal("Error", ex2.Message);

        // 3. Message and Inner
        var inner = new Exception("Inner");
        var ex3 = new InvalidArgumentValueException("Error", inner);
        Assert.Same(inner, ex3.InnerException);

        // 4. Detailed (The primary one used by SharpCliHost)
        var ex4 = new InvalidArgumentValueException("param", "val", typeof(int), inner);
        Assert.Equal("param", ex4.ArgumentName);
        Assert.Equal("val", ex4.ProvidedValue);
        Assert.Equal(typeof(int), ex4.ExpectedType);
    }

    [Fact]
    public void SerializationConstructor_FullData_SetsProperties()
    {
        // Arrange
        var info = new SerializationInfo(typeof(InvalidArgumentValueException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        info.AddValue("ArgumentName", "test-arg");
        info.AddValue("ProvidedValue", "test-val");
        info.AddValue("ExpectedType", typeof(bool));

        // Populate required base Exception fields for the constructor to run
        AddRequiredBaseFields(info);

        // Act
        var ex = InvokeSerializationConstructor(info, context);

        // Assert
        Assert.Equal("test-arg", ex.ArgumentName);
        Assert.Equal("test-val", ex.ProvidedValue);
        Assert.Equal(typeof(bool), ex.ExpectedType);
    }

    [Fact]
    public void SerializationConstructor_MissingData_UsesFallbackValues()
    {
        // This test specifically targets the '?? string.Empty' and '?? typeof(object)' branches
        // Arrange
        var info = new SerializationInfo(typeof(InvalidArgumentValueException), new FormatterConverter());
        var context = new StreamingContext(StreamingContextStates.All);

        // We explicitly add nulls for the keys to trigger the coalescing logic (??)
        info.AddValue("ArgumentName", null, typeof(string));
        info.AddValue("ProvidedValue", null, typeof(string));
        info.AddValue("ExpectedType", null, typeof(Type));

        AddRequiredBaseFields(info);

        // Act
        var ex = InvokeSerializationConstructor(info, context);

        // Assert
        Assert.Equal(string.Empty, ex.ArgumentName);
        Assert.Equal(string.Empty, ex.ProvidedValue);
        Assert.Equal(typeof(object), ex.ExpectedType);
    }

    // --- Reflection Helpers to access the protected constructor ---

    private InvalidArgumentValueException InvokeSerializationConstructor(SerializationInfo info,
        StreamingContext context)
    {
        var constructor = typeof(InvalidArgumentValueException).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null, [typeof(SerializationInfo), typeof(StreamingContext)], null);

        return (InvalidArgumentValueException)constructor!.Invoke([info, context]);
    }

    private void AddRequiredBaseFields(SerializationInfo info)
    {
        info.AddValue("ClassName", "InvalidArgumentValueException");
        info.AddValue("Message", "msg");
        info.AddValue("Data", null);
        info.AddValue("InnerException", null);
        info.AddValue("HelpURL", null);
        info.AddValue("StackTraceString", null);
        info.AddValue("RemoteStackTraceString", null);
        info.AddValue("RemoteStackIndex", 0);
        info.AddValue("ExceptionMethod", null);
        info.AddValue("HResult", 0);
        info.AddValue("Source", null);
    }

    [Fact]
    public async Task ShowHelpAsync_WithDescription_OutputsFullHeader()
    {
        // Arrange
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "A cool CLI description", sw);

        // Act
        await host.RunAsync([]); // Triggers ShowHelpAsync

        // Assert
        var output = sw.ToString();
        Assert.Contains("TestApp - A cool CLI description", output);
        Assert.Contains("USAGE:", output);
    }

    [Fact]
    public async Task ShowHelpAsync_NoDescription_OutputsNameOnly()
    {
        // Arrange
        await using var sw = new StringWriter();
        // Passing empty string for description
        var host = new SharpCliHost("MinimalApp", "", sw);

        // Act
        await host.RunAsync(["--help"]);

        // Assert
        var output = sw.ToString();
        Assert.Contains("MinimalApp", output);
        Assert.DoesNotContain(" - ", output);
    }

    [Fact]
    public async Task ShowHelpAsync_WithCommands_OutputsSortedCommandsAndDescriptions()
    {
        // Arrange
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);

        // Registering out of alphabetical order to test sorting
        host.RegisterCommands(new HelpTestCommands());

        // Act
        await host.RunAsync(["help"]);

        // Assert
        var output = sw.ToString();
        Assert.Contains("COMMANDS:", output);

        // Verify Alphabetical Order (z-cmd should come after a-cmd)
        var indexA = output.IndexOf("a-cmd", StringComparison.Ordinal);
        var indexZ = output.IndexOf("z-cmd", StringComparison.Ordinal);
        Assert.True(indexA < indexZ, "Commands should be sorted alphabetically.");

        // Verify Descriptions are present
        Assert.Contains("a-cmd | Alpha description", output);
        Assert.Contains("z-cmd | Zebra description", output);
    }

    [Fact]
    public void CreateBuilder_ReturnsNewBuilderInstance()
    {
        // Act
        var builder = SharpCliHost.CreateBuilder();

        // Assert
        Assert.NotNull(builder);
        Assert.IsType<SharpCliHostBuilder>(builder);
    }

    [Fact]
    public void CreateBuilder_ProducesIndependentInstances()
    {
        // Act
        var builder1 = SharpCliHost.CreateBuilder();
        var builder2 = SharpCliHost.CreateBuilder();

        // Assert
        Assert.NotSame(builder1, builder2);
    }

    [Fact]
    public void CreateBuilder_AllowsFluentChainingImmediately()
    {
        // Act
        var host = SharpCliHost.CreateBuilder()
            .Name("TestApp")
            .Description("Testing Builder Factory")
            .Build();

        // Assert
        Assert.NotNull(host);

        var nameField = typeof(SharpCliHost).GetField("_name",
            BindingFlags.NonPublic |
            BindingFlags.Instance);

        Assert.Equal("TestApp", nameField?.GetValue(host));
    }

    [Fact]
    public async Task ShowHelpAsync_CommandWithoutDescription_IsStillVisible()
    {
        // Arrange
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);

        host.RegisterCommands(new UndocumentedCommands());

        // Act
        await host.RunAsync(["--help"]);

        // Assert
        var output = sw.ToString();
        Assert.Contains("COMMANDS:", output);

        // Verify the command name is present even if the description is missing
        Assert.Contains("undocumented-cmd", output);
        Assert.Contains("documented-cmd | Has a description", output);
    }

    [Fact]
    public async Task ShowHelpAsync_CustomHelpMessage_OverridesDefaultHelp()
    {
        // Arrange
        await using var sw = new StringWriter();
        var host = new SharpCliHost("TestApp", "Desc", sw);

        // Use reflection to set private _customHelpMessage for testing
        var field = typeof(SharpCliHost).GetField("_customHelpMessage",
            BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(host, "CUSTOM_OVERRIDE_MESSAGE");

        // Act
        await host.RunAsync(["--help"]);

        // Assert
        var output = sw.ToString();
        Assert.Equal("CUSTOM_OVERRIDE_MESSAGE\n", output.Replace("\r\n", "\n"));
        Assert.DoesNotContain("USAGE:", output);
    }

    [Fact]
    public async Task RunAsync_NegativeNumbers_ParsedAsArguments()
    {
        // Arrange
        string[] args = ["math", "-5", "--offset", "-10.5"];

        // Act
        var result = await _host.RunAsync(args);

        // Assert
        Assert.Equal(0, result);
        Assert.True(_mockCommands.WasExecuted);
        Assert.Equal(-5, _mockCommands.LastInt);
        Assert.Equal(-10.5, _mockCommands.LastDouble);
    }
    
    [Fact]
    public async Task RunAsync_PositionalNegativeNumber_ParsesCorrectly()
    {
        // Arrange
        string[] args = ["types", "-42", "3.14", "true", "High"];

        // Act
        await _host.RunAsync(args);

        // Assert
        Assert.Equal("-42|3.14|True|High", _mockCommands.LastValue);
    }
}