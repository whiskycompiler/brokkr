using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Brokkr.Location.Abstractions;
using Brokkr.Location.Converter;

namespace Brokkr.Location;

/// <summary>
/// Represents an absolute Windows file system path.
/// </summary>
[JsonConverter(typeof(LocationJsonConverterFactory))]
public partial record AbsoluteWindowsPath : AbsoluteLocalPath, IStaticLocationFactory<AbsoluteWindowsPath>
{
    /// <summary>
    /// Regex for detecting absolute Windows paths.
    /// </summary>
    [GeneratedRegex("""^[A-Za-z]:\\(?:[^<>:"/\\|?*\r\n]+\\)*[^<>:"/\\|?*\r\n]*$""", RegexOptions.Singleline)]
    private static partial Regex AbsoluteWindowsPathRegex();

    /// <summary>
    /// Initializes a new instance of the <see cref="AbsoluteWindowsPath"/> class.
    /// </summary>
    /// <param name="locationString">The absolute Windows path string.</param>
    /// <exception cref="ArgumentException">Thrown when the location string is not a valid absolute Windows path.</exception>
    public AbsoluteWindowsPath(string locationString)
    {
        if (!AbsoluteWindowsPathRegex().IsMatch(locationString))
        {
            throw new ArgumentException(
                "Location string is not a valid absolute windows path.",
                nameof(locationString));
        }

        LocationString = locationString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbsoluteWindowsPath"/> class.
    /// </summary>
    protected AbsoluteWindowsPath()
    {
    }

    /// <summary>
    /// Attempts to create an <see cref="AbsoluteWindowsPath"/> from the specified location string.
    /// </summary>
    /// <param name="locationString">The string to parse as an absolute Windows path.</param>
    /// <param name="location">When this method returns, contains the AbsoluteWindowsPath if the conversion succeeded, or null if the conversion failed.</param>
    /// <returns>true if <paramref name="locationString"/> was converted successfully; otherwise, false.</returns>
    public static bool TryCreate(string locationString, [NotNullWhen(true)] out AbsoluteWindowsPath? location)
    {
        if (AbsoluteWindowsPathRegex().IsMatch(locationString))
        {
            location = new AbsoluteWindowsPath
            {
                LocationString = locationString,
            };

            return true;
        }

        location = null;
        return false;
    }

    /// <inheritdoc/>
    public override AbsoluteWindowsPath Combine(string relativePath)
    {
        return new AbsoluteWindowsPath(Path.Combine(LocationString, relativePath));
    }

    /// <inheritdoc/>
    public override AbsoluteWindowsPath Combine(RelativeLocalPath relativeLocalPath)
    {
        return new AbsoluteWindowsPath(Path.Combine(LocationString, relativeLocalPath.LocationString));
    }
}
