using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Brokkr.Location.Abstractions;
using Brokkr.Location.Converter;

namespace Brokkr.Location;

/// <summary>
/// Represents a relative Windows file system path.
/// </summary>
[JsonConverter(typeof(LocationJsonConverterFactory))]
public partial record RelativeWindowsPath : RelativeLocalPath, IStaticLocationFactory<RelativeWindowsPath>
{
    /// <summary>
    /// Regex for detecting relative Windows paths.
    /// </summary>
    [GeneratedRegex(
        """^(?![A-Za-z]:\\)(?!\\\\)(?!\\)[^<>:"/\\|?*\r\n]+(?:\\[^<>:"/\\|?*\r\n]+)*\\?$""",
        RegexOptions.Singleline)]
    private static partial Regex RelativeWindowsPathRegex();

    /// <summary>
    /// Initializes a new instance of the <see cref="RelativeWindowsPath"/> class.
    /// </summary>
    /// <param name="locationString">The relative Windows path string.</param>
    /// <exception cref="ArgumentException">Thrown when the location string is not a valid relative Windows path.</exception>
    public RelativeWindowsPath(string locationString)
    {
        if (!RelativeWindowsPathRegex().IsMatch(locationString))
        {
            throw new ArgumentException(
                "Location string is not a valid relative windows path.",
                nameof(locationString));
        }

        LocationString = locationString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelativeWindowsPath"/> class.
    /// </summary>
    protected RelativeWindowsPath()
    {
    }

    /// <summary>
    /// Attempts to create a <see cref="RelativeWindowsPath"/> from the specified location string.
    /// </summary>
    /// <param name="locationString">The string to parse as a relative Windows path.</param>
    /// <param name="location">When this method returns, contains the RelativeWindowsPath if the conversion succeeded, or null if the conversion failed.</param>
    /// <returns>true if <paramref name="locationString"/> was converted successfully; otherwise, false.</returns>
    public static bool TryCreate(string locationString, [NotNullWhen(true)] out RelativeWindowsPath? location)
    {
        if (RelativeWindowsPathRegex().IsMatch(locationString))
        {
            location = new RelativeWindowsPath
            {
                LocationString = locationString,
            };
            return true;
        }

        location = null;
        return false;
    }
    
    /// <inheritdoc/>
    public override RelativeWindowsPath Combine(string relativePath)
    {
        return new RelativeWindowsPath(Path.Combine(LocationString, relativePath));
    }

    /// <inheritdoc/>
    public override RelativeWindowsPath Combine(RelativeLocalPath relativeLocalPath)
    {
        return new RelativeWindowsPath(Path.Combine(LocationString, relativeLocalPath.LocationString));
    }
}