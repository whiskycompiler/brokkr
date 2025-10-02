using System.Text.Json;

namespace Brokkr.Location.UnitTests;

public class RelativeWindowsPathTests
{
    [Theory]
    [MemberData(nameof(RelativeWindowsLocationTestData.AllValidLocations),
        MemberType = typeof(RelativeWindowsLocationTestData))]
    public void TryCreate_GivenValidLocation_ReturnsTrue(string locationString)
    {
        // act
        var result = RelativeWindowsPath.TryCreate(locationString, out var location);

        // assert
        Assert.True(result);
        Assert.NotNull(location);
        Assert.Equal(locationString, location.LocationString);
    }

    [Theory]
    [MemberData(nameof(RelativeWindowsLocationTestData.AllInvalidLocations),
        MemberType = typeof(RelativeWindowsLocationTestData))]
    public void TryCreate_GivenInvalidLocation_ReturnsFalse(string locationString)
    {
        // act
        var result = RelativeWindowsPath.TryCreate(locationString, out var location);

        // assert
        Assert.False(result);
        Assert.Null(location);
    }
    
    [Theory]
    [MemberData(nameof(RelativeWindowsLocationTestData.AllValidLocations),
        MemberType = typeof(RelativeWindowsLocationTestData))]
    public void JsonConversion_GivenValidLocation_Succeeds(string locationString)
    {
        var location = new RelativeWindowsPath(locationString);
        
        // act
        var json = JsonSerializer.Serialize(location);
        var result = JsonSerializer.Deserialize<RelativeWindowsPath>(json);

        // assert
        Assert.Equal(JsonSerializer.Serialize(locationString), json);
        Assert.NotNull(result);
        Assert.Equal(locationString, result.LocationString);
    }
    
    [Theory]
    [MemberData(nameof(RelativeWindowsLocationTestData.AllInvalidLocations),
        MemberType = typeof(RelativeWindowsLocationTestData))]
    public void JsonConversion_GivenInvalidLocation_ThrowsJsonException(string locationString)
    {
        // act
        var json = JsonSerializer.Serialize(locationString);
        void Act() => JsonSerializer.Deserialize<RelativeWindowsPath>(json);

        // assert
        Assert.Throws<JsonException>(Act);
    }
}

public static class RelativeWindowsLocationTestData
{
    public static IEnumerable<object[]> AllValidLocations =>
        SharedLocationTestData.RelativeWindowsPaths
            .Select(s => new object[]
            {
                s.LocationString,
            });

    public static IEnumerable<object[]> AllInvalidLocations =>
        SharedLocationTestData.AbsoluteWindowsPaths
            .Concat(SharedLocationTestData.InvalidAbsoluteUnixPaths)
            .Concat(SharedLocationTestData.InvalidRelativeUnixPaths)
            .Concat(SharedLocationTestData.AbsoluteUnixPaths)
            .Concat(SharedLocationTestData.RelativeUnixPaths)
            .Concat(SharedLocationTestData.Urls)
            .Select(s => new object[]
            {
                s.LocationString,
            });
}