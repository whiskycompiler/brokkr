using System.Text.Json;
using System.Text.Json.Serialization;

using Brokkr.Core.Converters;

using JetBrains.Annotations;

namespace Brokkr.Core.UnitTests.Converters;

public class PlainEnumStringConverterTests
{
    [Theory]
    [InlineData(TestStatus.Active, "Active")]
    [InlineData(TestStatus.Inactive, "Inactive")]
    [InlineData(TestStatus.Pending, "Pending")]
    public void Serialize_ObjectWithDefinedEnumProperty_SerializesEnumAsString(
        TestStatus status,
        string expectedValue)
    {
        // Arrange
        var testObject = new TestClassWithEnum
        {
            Status = status,
            Name = "Test",
        };

        // Act
        var json = JsonSerializer.Serialize(testObject);

        // Assert
        Assert.Contains($"\"Status\":\"{expectedValue}\"", json);
        Assert.Contains("\"Name\":\"Test\"", json);
    }

    [Fact]
    public void Serialize_ObjectWithUndefinedEnumValue_SerializesEnumAsNull()
    {
        // Arrange
        var testObject = new TestClassWithEnum
        {
            Status = (TestStatus)999,
            Name = "Test",
        };

        // Act
        var json = JsonSerializer.Serialize(testObject);

        // Assert
        Assert.Contains("\"Status\":null", json);
        Assert.Contains("\"Name\":\"Test\"", json);
    }

    [Fact]
    public void Serialize_ObjectWithFlagsEnum_SerializesUndefinedFlagCombinationsAsNull()
    {
        // Arrange
        var testObject = new TestClassWithFlagsEnum
        {
            Permissions = TestPermissions.Read | TestPermissions.Write,
            Description = "Test permissions",
        };

        // Act
        var json = JsonSerializer.Serialize(testObject);

        // Assert
        Assert.Contains("\"Permissions\":null", json);
        Assert.Contains("\"Description\":\"Test permissions\"", json);
    }
    
    [Fact]
    public void Serialize_ObjectWithFlagsEnum_SerializesDefinedFlagCombinationsAsString()
    {
        // Arrange
        var testObject = new TestClassWithFlagsEnum
        {
            Permissions = TestPermissions.Read | TestPermissions.Write | TestPermissions.Execute,
            Description = "Test permissions",
        };

        // Act
        var json = JsonSerializer.Serialize(testObject);

        // Assert
        Assert.Contains("\"Permissions\":\"All\"", json);
        Assert.Contains("\"Description\":\"Test permissions\"", json);
    }

    [Fact]
    public void Serialize_ObjectWithSingleFlagValue_SerializesCorrectly()
    {
        // Arrange
        var testObject = new TestClassWithFlagsEnum
        {
            Permissions = TestPermissions.Execute,
            Description = "Execute only",
        };

        // Act
        var json = JsonSerializer.Serialize(testObject);

        // Assert
        Assert.Contains("\"Permissions\":\"Execute\"", json);
    }

    [Fact]
    public void Serialize_ObjectWithUndefinedFlagsValue_SerializesAsNull()
    {
        // Arrange
        var testObject = new TestClassWithFlagsEnum
        {
            Permissions = (TestPermissions)999,
            Description = "Invalid permissions",
        };

        // Act
        var json = JsonSerializer.Serialize(testObject);

        // Assert
        Assert.Contains("\"Permissions\":null", json);
    }

    [Fact]
    public void Deserialize_JsonWithEnumProperty_ThrowsNotSupportedException()
    {
        // Arrange
        var json = """{"Status":"Active","Name":"Test"}""";

        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<TestClassWithEnum>(json));
        Assert.Contains("Operation is not supported because the specified enums type cannot be known!", exception.Message);
    }

    [Fact]
    public void Deserialize_JsonWithFlagsEnumProperty_ThrowsNotSupportedException()
    {
        // Arrange
        var json = """{"Permissions":"Read, Write","Description":"Test"}""";

        // Act & Assert
        var exception =
            Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<TestClassWithFlagsEnum>(json));

        Assert.Contains("Operation is not supported because the specified enums type cannot be known!", exception.Message);
    }

    [Fact]
    public void Serialize_ObjectWithNullEnumProperty_SerializesAsNull()
    {
        // Arrange
        var testObject = new TestClassWithNullableEnum
        {
            OptionalStatus = null,
            Name = "Test",
        };

        // Act
        var json = JsonSerializer.Serialize(testObject);

        // Assert
        Assert.Contains("\"OptionalStatus\":null", json);
        Assert.Contains("\"Name\":\"Test\"", json);
    }

    [Fact]
    public void Serialize_ObjectWithNullableEnumValueThatIsDefined_SerializesAsString()
    {
        // Arrange
        var testObject = new TestClassWithNullableEnum
        {
            OptionalStatus = TestStatus.Pending,
            Name = "Test",
        };

        // Act
        var json = JsonSerializer.Serialize(testObject);

        // Assert
        Assert.Contains("\"OptionalStatus\":\"Pending\"", json);
        Assert.Contains("\"Name\":\"Test\"", json);
    }

    private sealed class TestClassWithEnum
    {
        [JsonConverter(typeof(PlainEnumStringConverter))]
        public Enum Status { get; set; } = TestStatus.Active;

        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestClassWithFlagsEnum
    {
        [JsonConverter(typeof(PlainEnumStringConverter))]
        public Enum Permissions { get; set; } = TestPermissions.None;

        public string Description { get; set; } = string.Empty;
    }

    private sealed class TestClassWithNullableEnum
    {
        [JsonConverter(typeof(PlainEnumStringConverter))]
        public Enum? OptionalStatus { [UsedImplicitly] get; set; }

        public string Name { [UsedImplicitly] get; set; } = string.Empty;
    }

    public enum TestStatus
    {
        Active,
        Inactive,
        Pending,
    }

    [Flags]
    private enum TestPermissions
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4,
        All = Read | Write | Execute,
    }
}
