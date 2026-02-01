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
}

/// <summary>
/// Extension methods for <see cref="LocalPath"/>.
/// </summary>
public static class LocalPathExtensions
{
    extension(LocalPath)
    {
        /// <summary>
        /// Creates a <see cref="LocalPath"/> from the specified location string.
        /// </summary>
        /// <param name="locationString">The location string to parse.</param>
        /// <returns>A new <see cref="LocalPath"/> instance.</returns>
        /// <exception cref="InvalidCastException">
        ///     Thrown when the location string is not a local path but some other type of location.
        /// </exception>
        public static LocalPath Create(string locationString)
        {
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
            var result = Location.LocalPathLocationFactory.TryCreate(locationString, out var loc);
            location = (LocalPath?)loc;
            return result;
        }
    }
}
