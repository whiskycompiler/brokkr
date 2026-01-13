using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Brokkr.Location.Converter;

namespace Brokkr.Location.Abstractions;

/// <summary>
/// Base record for local paths.
/// </summary>
[JsonConverter(typeof(LocationJsonConverterFactory))]
public abstract record LocalPath : LocationBase
{
    /// <summary>
    /// Combines the path with another relative path.
    /// </summary>
    public abstract LocalPath Combine(string relativePath);

    /// <summary>
    /// Combines the path with another relative path.
    /// </summary>
    public abstract LocalPath Combine(RelativeLocalPath relativeLocalPath);

    /// <summary>
    /// Creates a <see cref="LocalPath"/> from the specified location string.
    /// </summary>
    /// <param name="locationString">The location string to parse.</param>
    /// <returns>A new <see cref="LocalPath"/> instance.</returns>
    public static LocalPath Create(string locationString)
    {
#pragma warning disable S1135
        // TODO: make these extensions when .NET 10 is released
#pragma warning restore S1135
        return (LocalPath)Location.LocalPathLocationFactory.Create(locationString);
    }

    /// <summary>
    /// Attempts to create a <see cref="LocalPath"/> from the specified location string.
    /// </summary>
    /// <param name="locationString">The location string to parse.</param>
    /// <param name="location">When this method returns, contains the created <see cref="LocalPath"/> if successful; otherwise, null.</param>
    /// <returns>true if the <paramref name="location"/> was successfully created; otherwise, false.</returns>
    public static bool TryCreate(string locationString, [NotNullWhen(true)] out LocalPath? location)
    {
#pragma warning disable S1135
        // TODO: make these extensions when .NET 10 is released
#pragma warning restore S1135
        var result = Location.LocalPathLocationFactory.TryCreate(locationString, out var loc);
        location = (LocalPath?)loc;
        return result;
    }
}
