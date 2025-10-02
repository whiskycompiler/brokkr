using System.Text.Json;

namespace Brokkr.Location.UnitTests;

public class AbsoluteUnixPathTests
{
    [Theory]
    [MemberData(nameof(AbsoluteUnixLocationTestData.AllValidLocations),
        MemberType = typeof(AbsoluteUnixLocationTestData))]
    public void TryCreate_GivenValidLocation_ReturnsTrue(string locationString)
    {
        // act
        var result = AbsoluteUnixPath.TryCreate(locationString, out var location);

        // assert
        Assert.True(result);
        Assert.NotNull(location);
        Assert.Equal(locationString, location.LocationString);
    }

    [Theory]
    [MemberData(nameof(AbsoluteUnixLocationTestData.AllInvalidLocations),
        MemberType = typeof(AbsoluteUnixLocationTestData))]
    public void TryCreate_GivenInvalidLocation_ReturnsFalse(string locationString)
    {
        // act
        var result = AbsoluteUnixPath.TryCreate(locationString, out var location);

        // assert
        Assert.False(result);
        Assert.Null(location);
    }
    
    [Theory]
    [MemberData(nameof(AbsoluteUnixLocationTestData.AllValidLocations),
        MemberType = typeof(AbsoluteUnixLocationTestData))]
    public void JsonConversion_GivenValidLocation_Succeeds(string locationString)
    {
        var location = new AbsoluteUnixPath(locationString);
        
        // act
        var json = JsonSerializer.Serialize(location);
        var result = JsonSerializer.Deserialize<AbsoluteUnixPath>(json);

        // assert
        Assert.Equal(JsonSerializer.Serialize(locationString), json);
        Assert.NotNull(result);
        Assert.Equal(locationString, result.LocationString);
    }
    
    [Theory]
    [MemberData(nameof(AbsoluteUnixLocationTestData.AllInvalidLocations),
        MemberType = typeof(AbsoluteUnixLocationTestData))]
    public void JsonConversion_GivenInvalidLocation_ThrowsJsonException(string locationString)
    {
        // act
        var json = JsonSerializer.Serialize(locationString);
        void Act() => JsonSerializer.Deserialize<AbsoluteUnixPath>(json);

        // assert
        Assert.Throws<JsonException>(Act);
    }
}

public static class AbsoluteUnixLocationTestData
{
    public static IEnumerable<object[]> AllValidLocations =>
        SharedLocationTestData.AbsoluteUnixPaths
            .Select(s => new object[]
            {
                s.LocationString,
            });

    public static IEnumerable<object[]> AllInvalidLocations =>
        SharedLocationTestData.InvalidAbsoluteUnixPaths
            .Concat(SharedLocationTestData.RelativeUnixPaths)
            .Concat(SharedLocationTestData.InvalidRelativeUnixPaths)
            .Concat(SharedLocationTestData.AbsoluteWindowsPaths)
            .Concat(SharedLocationTestData.RelativeWindowsPaths)
            .Concat(SharedLocationTestData.Urls)
            .Select(s => new object[]
            {
                s.LocationString,
            });
}
