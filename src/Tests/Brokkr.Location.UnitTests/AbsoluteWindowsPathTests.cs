using System.Text.Json;

namespace Brokkr.Location.UnitTests;

public class AbsoluteWindowsPathTests
{
    [Theory]
    [MemberData(nameof(AbsoluteWindowsLocationTestData.AllValidLocations),
        MemberType = typeof(AbsoluteWindowsLocationTestData))]
    public void TryCreate_GivenValidLocation_ReturnsTrue(string locationString)
    {
        // act
        var result = AbsoluteWindowsPath.TryCreate(locationString, out var location);

        // assert
        Assert.True(result);
        Assert.NotNull(location);
        Assert.Equal(locationString, location.LocationString);
    }

    [Theory]
    [MemberData(nameof(AbsoluteWindowsLocationTestData.AllInvalidLocations),
        MemberType = typeof(AbsoluteWindowsLocationTestData))]
    public void TryCreate_GivenInvalidLocation_ReturnsFalse(string locationString)
    {
        // act
        var result = AbsoluteWindowsPath.TryCreate(locationString, out var location);

        // assert
        Assert.False(result);
        Assert.Null(location);
    }
    
    [Theory]
    [MemberData(nameof(AbsoluteWindowsLocationTestData.AllValidLocations),
        MemberType = typeof(AbsoluteWindowsLocationTestData))]
    public void JsonConversion_GivenValidLocation_Succeeds(string locationString)
    {
        var location = new AbsoluteWindowsPath(locationString);
        
        // act
        var json = JsonSerializer.Serialize(location);
        var result = JsonSerializer.Deserialize<AbsoluteWindowsPath>(json);

        // assert
        Assert.Equal(JsonSerializer.Serialize(locationString), json);
        Assert.NotNull(result);
        Assert.Equal(locationString, result.LocationString);
    }
    
    [Theory]
    [MemberData(nameof(AbsoluteWindowsLocationTestData.AllInvalidLocations),
        MemberType = typeof(AbsoluteWindowsLocationTestData))]
    public void JsonConversion_GivenInvalidLocation_ThrowsJsonException(string locationString)
    {
        // act
        var json = JsonSerializer.Serialize(locationString);
        void Act() => JsonSerializer.Deserialize<AbsoluteWindowsPath>(json);

        // assert
        Assert.Throws<JsonException>(Act);
    }
}

public static class AbsoluteWindowsLocationTestData
{
    public static IEnumerable<object[]> AllValidLocations =>
        SharedLocationTestData.AbsoluteWindowsPaths
            .Select(s => new object[]
            {
                s.LocationString,
            });

    public static IEnumerable<object[]> AllInvalidLocations =>
        SharedLocationTestData.RelativeWindowsPaths
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