using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Brokkr.Location.Abstractions;
using Brokkr.Location.Converter;

namespace Brokkr.Location;

/// <summary>
/// Provides static methods for creating locations.
/// </summary>
public static class Location
{
    /// <summary>
    /// Dictionary of JsonConverters to be automatically used for child classes of <see cref="LocationBase"/>.
    /// </summary>
    public static Dictionary<Type, JsonConverter> JsonConverters { get; } = new() {
        [typeof(LocationBase)] = new LocationBaseJsonConverter(),
        [typeof(LocalPath)] = new LocalPathJsonConverter(),
        [typeof(AbsoluteLocalPath)] = new AbsoluteLocalPathJsonConverter(),
        [typeof(AbsoluteWindowsPath)] = new ConcreteLocationJsonConverter<AbsoluteWindowsPath>(),
        [typeof(AbsoluteUnixPath)] = new ConcreteLocationJsonConverter<AbsoluteUnixPath>(),
        [typeof(RelativeLocalPath)] = new RelativeLocalPathJsonConverter(),
        [typeof(RelativeWindowsPath)] = new ConcreteLocationJsonConverter<RelativeWindowsPath>(),
        [typeof(RelativeUnixPath)] = new RelativeUnixPathJsonConverter(),
        [typeof(IndeterminateRelativePath)] = new ConcreteLocationJsonConverter<IndeterminateRelativePath>(),
        [typeof(Url)] = new ConcreteLocationJsonConverter<Url>(),
    };
    
    /// <summary>
    /// Gets or sets the location factory used to create locations.
    /// </summary>
    public static ILocationFactory LocationFactory { get; set; } = new DefaultLocationFactory();

    /// <summary>
    /// Creates a location from the specified location string.
    /// </summary>
    /// <param name="locationString">The location string to parse.</param>
    /// <returns>A new location instance.</returns>
    public static ILocation Create(string locationString)
    {
        return LocationFactory.Create(locationString);
    }

    /// <summary>
    /// Attempts to create a location from the specified location string.
    /// </summary>
    /// <param name="locationString">The location string to parse.</param>
    /// <param name="location">When this method returns, contains the created location if successful; otherwise, null.</param>
    /// <returns>true if the <paramref name="location"/> was successfully created; otherwise, false.</returns>
    public static bool TryCreate(string locationString, [NotNullWhen(true)] out ILocation? location)
    {
        return LocationFactory.TryCreate(locationString, out location);
    }
}
