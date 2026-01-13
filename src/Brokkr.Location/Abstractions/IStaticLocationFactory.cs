namespace Brokkr.Location.Abstractions;

/// <summary>
/// Describes a component with a static method to create concrete location instances.
/// </summary>
/// <typeparam name="TLocation">The type of location to create.</typeparam>
public interface IStaticLocationFactory<TLocation> where TLocation : ILocation
{
    /// <summary>
    /// Attempts to create a location from the specified location string.
    /// </summary>
    /// <param name="locationString">The location string to parse.</param>
    /// <param name="location">When this method returns, contains the created location if successful; otherwise, null.</param>
    /// <returns>true if the <paramref name="location"/> was successfully created; otherwise, false.</returns>
    static abstract bool TryCreate(string locationString, out TLocation? location);
}
