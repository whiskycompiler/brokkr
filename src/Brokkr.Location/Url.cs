using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Brokkr.Location.Abstractions;
using Brokkr.Location.Converter;

namespace Brokkr.Location;

/// <summary>
/// Represents a URL location.
/// </summary>
[JsonConverter(typeof(LocationJsonConverterFactory))]
public partial record Url : LocationBase, IStaticLocationFactory<Url>
{
    /// <summary>
    /// Regex for detecting URLs.
    /// </summary>
    [GeneratedRegex(
        """^(?:[a-zA-Z][a-zA-Z0-9+.-]*:)(?:\/\/(?:[^\s\/?#@]+@)?(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9\p{L}\p{N}-]+(?:\.[a-zA-Z0-9\p{L}\p{N}-]+)*)|localhost|\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}|\[(?:[0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}\])(?::\d+)?(?:\/[^\s]*)?|\/\/\/[a-zA-Z]:[\\\/][^\s]*|\/\/\/[^\s]*|\/\/[a-zA-Z]:[\\\/][^\s]*)$""",
        RegexOptions.Singleline)]
    private static partial Regex UrlRegex();

    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    /// <param name="locationString">The URL string.</param>
    /// <exception cref="ArgumentException">Thrown when the location string is not a valid URL.</exception>
    public Url(string locationString)
    {
        if (!UrlRegex().IsMatch(locationString))
        {
            throw new ArgumentException(
                "Location string is not a valid URL.",
                nameof(locationString));
        }

        LocationString = locationString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    protected Url()
    {
    }

    /// <summary>
    /// Attempts to create a <see cref="Url"/> from the specified location string.
    /// </summary>
    /// <param name="locationString">The string to parse as a URL.</param>
    /// <param name="location">When this method returns, contains the Url if the conversion succeeded, or null if the conversion failed.</param>
    /// <returns>true if <paramref name="locationString"/> was converted successfully; otherwise, false.</returns>
    public static bool TryCreate(string locationString, [NotNullWhen(true)] out Url? location)
    {
        if (UrlRegex().IsMatch(locationString))
        {
            location = new Url
            {
                LocationString = locationString,
            };
            return true;
        }

        location = null;
        return false;
    }
}