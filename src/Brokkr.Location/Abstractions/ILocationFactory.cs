using System.Diagnostics.CodeAnalysis;

namespace Brokkr.Location.Abstractions;

/// <summary>
/// Factory for creating location instances.
/// </summary>
public interface ILocationFactory
{
    /// <summary>
    /// Creates a location from the specified location string.
    /// </summary>
    /// <param name="locationString">The location string to parse.</param>
    /// <returns>A new location instance.</returns>
    ILocation Create(string locationString);

    /// <summary>
    /// Attempts to create a location from the specified location string.
    /// </summary>
    /// <param name="locationString">The location string to parse.</param>
    /// <param name="location">When this method returns, contains the created location if successful; otherwise, null.</param>
    /// <returns>true if the location was successfully created; otherwise, false.</returns>
    bool TryCreate(string locationString, [NotNullWhen(true)] out ILocation? location);
}