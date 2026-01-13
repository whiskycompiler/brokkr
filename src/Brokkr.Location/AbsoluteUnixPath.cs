using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Brokkr.Location.Abstractions;
using Brokkr.Location.Converter;

namespace Brokkr.Location;

/// <summary>
/// Represents an absolute Unix file system path.
/// </summary>
[JsonConverter(typeof(LocationJsonConverterFactory))]
public partial record AbsoluteUnixPath : AbsoluteLocalPath, IStaticLocationFactory<AbsoluteUnixPath>
{
    /// <summary>
    /// Regex for detecting absolute Unix paths.
    /// </summary>
    [GeneratedRegex(
        """^(/(?:[^/\0\p{C}]+|\.{1,2})(?:/(?:[^/\0\p{C}]+|\.{1,2}))*/?|/)$""",
        RegexOptions.Singleline)]
    private static partial Regex AbsoluteUnixPathRegex();

    /// <summary>
    /// Initializes a new instance of the <see cref="AbsoluteUnixPath"/> class.
    /// </summary>
    /// <param name="locationString">The absolute Unix path string.</param>
    /// <exception cref="ArgumentException">Thrown when the location string is not a valid absolute Unix path.</exception>
    public AbsoluteUnixPath(string locationString)
    {
        if (!AbsoluteUnixPathRegex().IsMatch(locationString))
        {
            throw new ArgumentException(
                "Location string is not a valid absolute unix path.",
                nameof(locationString));
        }

        LocationString = locationString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbsoluteUnixPath"/> class.
    /// </summary>
    protected AbsoluteUnixPath()
    {
    }

    /// <summary>
    /// Attempts to create an <see cref="AbsoluteUnixPath"/> from the specified location string.
    /// </summary>
    /// <param name="locationString">The string to parse as an absolute Unix path.</param>
    /// <param name="location">When this method returns, contains the AbsoluteUnixPath if the conversion succeeded, or null if the conversion failed.</param>
    /// <returns>true if <paramref name="locationString"/> was converted successfully; otherwise, false.</returns>
    public static bool TryCreate(string locationString, [NotNullWhen(true)] out AbsoluteUnixPath? location)
    {
        if (AbsoluteUnixPathRegex().IsMatch(locationString))
        {
            location = new AbsoluteUnixPath
            {
                LocationString = locationString,
            };

            return true;
        }

        location = null;
        return false;
    }

    /// <inheritdoc/>
    public override AbsoluteUnixPath Combine(string relativePath)
    {
        return new AbsoluteUnixPath(Path.Combine(LocationString, relativePath));
    }

    /// <inheritdoc/>
    public override AbsoluteUnixPath Combine(RelativeLocalPath relativeLocalPath)
    {
        return new AbsoluteUnixPath(Path.Combine(LocationString, relativeLocalPath.LocationString));
    }
}
