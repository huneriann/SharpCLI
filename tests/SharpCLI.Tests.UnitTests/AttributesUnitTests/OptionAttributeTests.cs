namespace SharpCLI.Tests.UnitTests.AttributesUnitTests;

public class OptionAttributeTests
{
    [Fact]
    public void Constructor_WithNames_SetsShortAndLongName()
    {
        // Arrange
        const string shortName = "v";
        const string longName = "verbose";

        // Act
        var attribute = new OptionAttribute(shortName, longName);

        // Assert
        Assert.Equal(shortName, attribute.ShortName);
        Assert.Equal(longName, attribute.LongName);
        Assert.Equal(string.Empty, attribute.Description);
        Assert.Null(attribute.DefaultValue);
    }

    [Fact]
    public void Description_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var attribute = new OptionAttribute("o", "output");
        const string description = "The output directory path.";

        // Act
        attribute.Description = description;

        // Assert
        Assert.Equal(description, attribute.Description);
    }

    [Fact]
    public void DefaultValue_SetAndGet_WorksWithVariousTypes()
    {
        // Arrange
        var attribute = new OptionAttribute("t", "timeout");
        const int timeoutValue = 30;

        // Act
        attribute.DefaultValue = timeoutValue;

        // Assert
        Assert.Equal(timeoutValue, attribute.DefaultValue);
    }

    [Fact]
    public void AttributeUsage_IsTargetedToParameters()
    {
        // Arrange & Act
        var usageAttribute = typeof(OptionAttribute).GetCustomAttribute<AttributeUsageAttribute>();

        // Assert
        Assert.NotNull(usageAttribute);
        Assert.Equal(AttributeTargets.Parameter, usageAttribute.ValidOn);
    }

    [Fact]
    public void ObjectInitializer_SetsAllPropertiesCorrectly()
    {
        // Arrange & Act
        var attribute = new OptionAttribute("f", "force")
        {
            Description = "Force the operation",
            DefaultValue = true
        };

        // Assert
        Assert.Equal("f", attribute.ShortName);
        Assert.Equal("force", attribute.LongName);
        Assert.Equal("Force the operation", attribute.Description);
        Assert.True((bool?)attribute.DefaultValue);
    }

    [Fact]
    public void DefaultValue_CanBeSetToNull()
    {
        // Arrange
        var attribute = new OptionAttribute("c", "config") { DefaultValue = "some/path" };

        // Act
        attribute.DefaultValue = null;

        // Assert
        Assert.Null(attribute.DefaultValue);
    }
}