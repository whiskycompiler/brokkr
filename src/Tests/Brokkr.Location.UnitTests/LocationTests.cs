using System.Text.Json;

using Brokkr.Location.Abstractions;

namespace Brokkr.Location.UnitTests;

public class LocationTests
{
    [Theory]
    [MemberData(nameof(LocationTestData.AllValidTestLocations), MemberType = typeof(LocationTestData))]
    public void TryCreate_GivenValidLocation_ReturnsTrue(string locationString, Type expectedLocationType)
    {
        // act
        var result = Location.TryCreate(locationString, out var location);

        // assert
        Assert.True(result);
        Assert.Equal(expectedLocationType, location!.GetType());
        Assert.Equal(locationString, location.LocationString);
    }

    [Theory]
    [MemberData(nameof(LocationTestData.AllInvalidTestLocations), MemberType = typeof(LocationTestData))]
    public void TryCreate_GivenInvalidLocation_ReturnsFalse(string locationString)
    {
        // act
        var result = Location.TryCreate(locationString, out var location);

        // assert
        Assert.False(result);
        Assert.Null(location);
    }

    [Theory]
    [MemberData(nameof(LocationTestData.AllValidTestLocations),
        MemberType = typeof(LocationTestData))]
    public void JsonConversion_GivenValidLocation_Succeeds(string locationString, Type expectedLocationType)
    {
        // act
        var json = JsonSerializer.Serialize(locationString);
        var result = JsonSerializer.Deserialize<LocationBase>(json);

        // assert
        Assert.Equal(JsonSerializer.Serialize(locationString), json);
        Assert.NotNull(result);
        Assert.Equal(locationString, result.LocationString);
        Assert.Equal(expectedLocationType, result.GetType());
    }

    [Theory]
    [MemberData(nameof(LocationTestData.AllInvalidTestLocations),
        MemberType = typeof(LocationTestData))]
    public void JsonConversion_GivenInvalidLocation_ThrowsJsonException(string locationString)
    {
        // act
        var json = JsonSerializer.Serialize(locationString);

        void Act()
        {
            JsonSerializer.Deserialize<LocationBase>(json);
        }

        // assert
        Assert.Throws<JsonException>(Act);
    }
}

public static class LocationTestData
{
    public static IEnumerable<object[]> AllValidTestLocations
        => SharedLocationTestData.IndeterminateRelativePaths
            .Concat(SharedLocationTestData.AbsoluteUnixPaths)
            .Concat(SharedLocationTestData.RelativeUnixPaths)
            .Concat(SharedLocationTestData.AbsoluteWindowsPaths)
            .Concat(SharedLocationTestData.RelativeWindowsPaths)
            .Concat(SharedLocationTestData.Urls)
            .Select(s => new object[] { s.LocationString, s.ExpectedLocationType });

    public static IEnumerable<object[]> AllInvalidTestLocations
        => SharedLocationTestData.InvalidAbsoluteUnixPaths
            .Concat(SharedLocationTestData.InvalidRelativeUnixPaths)
            .Select(s => new object[] { s.LocationString });
}
