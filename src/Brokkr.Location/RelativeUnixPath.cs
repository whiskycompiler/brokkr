using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Brokkr.Location.Abstractions;
using Brokkr.Location.Converter;

namespace Brokkr.Location;

/// <summary>
/// Represents a relative path in Unix format.
/// </summary>
[JsonConverter(typeof(LocationJsonConverterFactory))]
public partial record RelativeUnixPath : RelativeLocalPath, IStaticLocationFactory<RelativeUnixPath>
{
    /// <summary>
    /// Regex for detecting relative Unix paths.
    /// </summary>
    [GeneratedRegex(
        @"^(?:\.\.?\/|[^/\0\p{C}]+\/)*(?:[^/\0\p{C}]+)?$",
        RegexOptions.Singleline)]
    private static partial Regex RelativeUnixPathRegex();

    /// <summary>
    /// Regex for detecting relative Unix paths but only if they do not contain a backslash.
    /// </summary>
    [GeneratedRegex(
        """^(?:\.\.?\/|[^/\0\\\p{C}]+\/)*(?:[^/\0\\\p{C}]+)?$""",
        RegexOptions.Singleline)]
    private static partial Regex RelativeUnixPathRegexWithoutBackslash();

    /// <summary>
    /// Initializes a new instance of the <see cref="RelativeUnixPath"/> class.
    /// </summary>
    /// <param name="locationString">The location string representing a relative Unix path.</param>
    /// <param name="disallowBackslash">If true, backslashes are not allowed in the path.</param>
    /// <exception cref="ArgumentException">Thrown when the location string is not a valid relative Unix path.</exception>
    public RelativeUnixPath(string locationString, bool disallowBackslash = false)
    {
        if (disallowBackslash
            ? !RelativeUnixPathRegexWithoutBackslash().IsMatch(locationString)
            : !RelativeUnixPathRegex().IsMatch(locationString))
        {
            throw new ArgumentException(
                "Location string is not a valid relative unix path.",
                nameof(locationString));
        }

        LocationString = locationString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelativeUnixPath"/> class.
    /// </summary>
    protected RelativeUnixPath()
    {
    }

    /// <summary>
    /// Attempts to create a new instance of <see cref="RelativeUnixPath"/> from the specified location string.
    /// </summary>
    /// <param name="locationString">The location string to parse.</param>
    /// <param name="location">When this method returns, contains the created location if successful; otherwise, null.</param>
    /// <param name="disallowBackslash">If true, backslashes are not allowed in the path.</param>
    /// <returns>True if the <paramref name="location"/> was successfully created; otherwise, false.</returns>
    public static bool TryCreate(
        string locationString,
        [NotNullWhen(true)] out RelativeUnixPath? location,
        bool disallowBackslash)
    {
        if (disallowBackslash
            ? RelativeUnixPathRegexWithoutBackslash().IsMatch(locationString)
            : RelativeUnixPathRegex().IsMatch(locationString))
        {
            location = new RelativeUnixPath
            {
                LocationString = locationString,
            };

            return true;
        }

        location = null;
        return false;
    }

    /// <summary>
    /// Attempts to create a new instance of <see cref="RelativeUnixPath"/> from the specified location string.
    /// </summary>
    /// <param name="locationString">The location string to parse.</param>
    /// <param name="location">When this method returns, contains the created location if successful; otherwise, null.</param>
    /// <returns>True if the <paramref name="location"/> was successfully created; otherwise, false.</returns>
    public static bool TryCreate(string locationString, [NotNullWhen(true)] out RelativeUnixPath? location)
    {
        return TryCreate(locationString, out location, true);
    }

    /// <inheritdoc/>
    public override RelativeUnixPath Combine(string relativePath)
    {
        return new RelativeUnixPath(Path.Combine(LocationString, relativePath), true);
    }

    /// <inheritdoc/>
    public override RelativeUnixPath Combine(RelativeLocalPath relativeLocalPath)
    {
        return new RelativeUnixPath(Path.Combine(LocationString, relativeLocalPath.LocationString), true);
    }
}
