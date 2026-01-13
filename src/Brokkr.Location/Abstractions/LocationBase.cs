using System.Text.Json.Serialization;

using Brokkr.Location.Converter;

namespace Brokkr.Location.Abstractions;

/// <summary>
/// Base record for location implementations.
/// </summary>
[JsonConverter(typeof(LocationJsonConverterFactory))]
public abstract record LocationBase : ILocation
{
    /// <summary>
    /// Gets the location string representation.
    /// </summary>
    public string LocationString { get; protected init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationBase"/> class.
    /// </summary>
    protected LocationBase()
    {
        LocationString = string.Empty;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return LocationString;
    }

    /// <summary>
    /// Provides implicit conversion from <see cref="LocationBase"/> to <see cref="string"/>.
    /// </summary>
    public static implicit operator string(LocationBase location)
    {
        return location.LocationString;
    }
}
