namespace Brokkr.Location.Abstractions;

/// <summary>
/// Describes a location object that can be a URI, absolute path, or relative path.
/// </summary>
public interface ILocation
{
    /// <summary>
    /// Gets the string representation of the location.
    /// </summary>
    string LocationString { get; }
}
