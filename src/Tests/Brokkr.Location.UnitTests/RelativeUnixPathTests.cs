using System.Text.Json;

namespace Brokkr.Location.UnitTests;

public class RelativeUnixPathTests
{
    [Theory]
    [MemberData(nameof(RelativeUnixLocationTestData.LimitedValidLocations),
        MemberType = typeof(RelativeUnixLocationTestData))]
    public void TryCreate_GivenValidLocationDisallowingBackslash_ReturnsTrue(string locationString)
    {
        // act
        var result1 = RelativeUnixPath.TryCreate(locationString, out var location1);
        var result2 = RelativeUnixPath.TryCreate(locationString, out var location2, disallowBackslash: true);

        // assert
        Assert.Equal(result1, result2);
        Assert.Equal(location1, location2);

        Assert.True(result1);
        Assert.NotNull(location1);
        Assert.Equal(locationString, location1.LocationString);
    }

    [Theory]
    [MemberData(nameof(RelativeUnixLocationTestData.AllValidLocations),
        MemberType = typeof(RelativeUnixLocationTestData))]
    public void TryCreate_GivenValidLocationAllowingBackslash_ReturnsTrue(string locationString)
    {
        // act
        var result = RelativeUnixPath.TryCreate(locationString, out var location, disallowBackslash: false);

        // assert
        Assert.True(result);
        Assert.NotNull(location);
        Assert.Equal(locationString, location.LocationString);
    }

    [Theory]
    [MemberData(nameof(RelativeUnixLocationTestData.LimitedInvalidLocations),
        MemberType = typeof(RelativeUnixLocationTestData))]
    public void TryCreate_GivenInvalidLocationAllowingBackslash_ReturnsFalse(string locationString)
    {
        // act
        var result1 = RelativeUnixPath.TryCreate(locationString, out var location1);
        var result2 = RelativeUnixPath.TryCreate(locationString, out var location2);

        // assert
        Assert.Equal(result1, result2);
        Assert.Equal(location1, location2);

        Assert.False(result1);
        Assert.Null(location1);
    }

    [Theory]
    [MemberData(nameof(RelativeUnixLocationTestData.ExtendedInvalidLocations),
        MemberType = typeof(RelativeUnixLocationTestData))]
    public void TryCreate_GivenInvalidLocationDisallowingBackslash_ReturnsFalse(string locationString)
    {
        // act
        var result = RelativeUnixPath.TryCreate(locationString, out var location, disallowBackslash: true);

        // assert
        Assert.False(result);
        Assert.Null(location);
    }

    [Theory]
    [MemberData(nameof(RelativeUnixLocationTestData.LimitedValidLocations),
        MemberType = typeof(RelativeUnixLocationTestData))]
    public void JsonConversion_GivenValidLocation_Succeeds(string locationString)
    {
        var location = new RelativeUnixPath(locationString);

        // act
        var json = JsonSerializer.Serialize(location);
        var result = JsonSerializer.Deserialize<RelativeUnixPath>(json);

        // assert
        Assert.Equal(JsonSerializer.Serialize(locationString), json);
        Assert.NotNull(result);
        Assert.Equal(locationString, result.LocationString);
    }

    [Theory]
    [MemberData(nameof(RelativeUnixLocationTestData.ExtendedInvalidLocations),
        MemberType = typeof(RelativeUnixLocationTestData))]
    public void JsonConversion_GivenInvalidLocation_ThrowsJsonException(string locationString)
    {
        // act
        var json = JsonSerializer.Serialize(locationString);

        void Act()
        {
            JsonSerializer.Deserialize<RelativeUnixPath>(json);
        }

        // assert
        Assert.Throws<JsonException>(Act);
    }
}

public static class RelativeUnixLocationTestData
{
    public static IEnumerable<object[]> AllValidLocations
        => SharedLocationTestData.IndeterminateRelativePaths
            .Concat(SharedLocationTestData.RelativeUnixPaths)
            .Concat(SharedLocationTestData.AbsoluteWindowsPaths) // theoretically valid file names
            .Concat(SharedLocationTestData.RelativeWindowsPaths) // theoretically valid file names
            .Select(s => new object[] { s.LocationString });

    public static IEnumerable<object[]> LimitedValidLocations
        => SharedLocationTestData.IndeterminateRelativePaths
            .Concat(SharedLocationTestData.RelativeUnixPaths)
            .Select(s => new object[] { s.LocationString });

    public static IEnumerable<object[]> LimitedInvalidLocations
        => SharedLocationTestData.AbsoluteUnixPaths
            .Concat(SharedLocationTestData.InvalidAbsoluteUnixPaths)
            .Concat(SharedLocationTestData.InvalidRelativeUnixPaths)
            .Concat(SharedLocationTestData.Urls)
            // windows paths are theoretically valid file names
            .Select(s => new object[] { s.LocationString });

    public static IEnumerable<object[]> ExtendedInvalidLocations
        => SharedLocationTestData.AbsoluteUnixPaths
            .Concat(SharedLocationTestData.InvalidAbsoluteUnixPaths)
            .Concat(SharedLocationTestData.InvalidRelativeUnixPaths)
            .Concat(SharedLocationTestData.AbsoluteWindowsPaths)
            .Concat(SharedLocationTestData.RelativeWindowsPaths)
            .Concat(SharedLocationTestData.Urls)
            .Select(s => new object[] { s.LocationString });
}
