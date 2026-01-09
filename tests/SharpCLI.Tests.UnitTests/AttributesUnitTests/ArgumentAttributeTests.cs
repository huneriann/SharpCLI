namespace SharpCLI.Tests.UnitTests.AttributesUnitTests;

public class ArgumentAttributeTests
{
    [Fact]
    public void Constructor_WithName_SetsNameAndDefaultProperties()
    {
        // Arrange
        const string expectedName = "filePath";

        // Act
        var attribute = new ArgumentAttribute(expectedName);

        // Assert
        Assert.Equal(expectedName, attribute.Name);
        Assert.Equal(string.Empty, attribute.Description);
        Assert.True(attribute.Required);
    }

    [Fact]
    public void Description_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var attribute = new ArgumentAttribute("input");
        const string expectedDescription = "The path to the input file.";

        // Act
        attribute.Description = expectedDescription;

        // Assert
        Assert.Equal(expectedDescription, attribute.Description);
    }

    [Fact]
    public void Required_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var attribute = new ArgumentAttribute("output");

        // Act
        attribute.Required = false;

        // Assert
        Assert.False(attribute.Required);
    }

    [Fact]
    public void AttributeUsage_IsTargetedToParameters()
    {
        // Arrange & Act
        var usageAttribute = typeof(ArgumentAttribute).GetCustomAttribute<AttributeUsageAttribute>();

        // Assert
        Assert.NotNull(usageAttribute);
        Assert.Equal(AttributeTargets.Parameter, usageAttribute.ValidOn);
    }

    [Fact]
    public void Properties_WithAllValuesSet_RetainsValues()
    {
        // Arrange & Act
        var attribute = new ArgumentAttribute("count")
        {
            Description = "Number of retries",
            Required = false
        };

        // Assert
        Assert.Equal("count", attribute.Name);
        Assert.Equal("Number of retries", attribute.Description);
        Assert.False(attribute.Required);
    }
}