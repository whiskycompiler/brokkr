using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Brokkr.Location.Abstractions;
using Brokkr.Location.Converter;

namespace Brokkr.Location;

/// <summary>
/// Represents a relative path that is indeterminate
/// (can be either Unix or Windows e.g. just a filename).
/// </summary>
[JsonConverter(typeof(LocationJsonConverterFactory))]
public partial record IndeterminateRelativePath : RelativeLocalPath, IStaticLocationFactory<IndeterminateRelativePath>
{
    /// <summary>
    /// Regex for detecting indeterminate paths.
    /// </summary>
    [GeneratedRegex(
        """^(?!.*[\\/]|[A-Za-z]:)[\p{L}\p{N}._\-\(\)\[\]]+(?:[\s\p{L}\p{N}._\-\(\)\[\]]+)*$""",
        RegexOptions.Singleline)]
    private static partial Regex IndeterminateRelativePathRegex();

    /// <summary>
    /// Initializes a new instance of the <see cref="AbsoluteUnixPath"/> class.
    /// </summary>
    /// <param name="locationString">The absolute Unix path string.</param>
    /// <exception cref="ArgumentException">Thrown when the location string is not a valid absolute Unix path.</exception>
    public IndeterminateRelativePath(string locationString)
    {
        if (!IndeterminateRelativePathRegex().IsMatch(locationString))
        {
            throw new ArgumentException(
                "Location string is not a valid indeterminate relative path.",
                nameof(locationString));
        }

        LocationString = locationString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndeterminateRelativePath"/> class.
    /// </summary>
    protected IndeterminateRelativePath()
    {
    }

    /// <summary>
    /// Attempts to create an indeterminate relative path from the specified location string.
    /// </summary>
    /// <param name="locationString">The location string to parse.</param>
    /// <param name="location">When this method returns, contains the created location if successful; otherwise, null.</param>
    /// <returns>true if the <paramref name="location"/> was successfully created; otherwise, false.</returns>
    public static bool TryCreate(string locationString, [NotNullWhen(true)] out IndeterminateRelativePath? location)
    {
        if (IndeterminateRelativePathRegex().IsMatch(locationString))
        {
            location = new IndeterminateRelativePath
            {
                LocationString = locationString,
            };

            return true;
        }

        location = null;
        return false;
    }

    /// <inheritdoc/>
    public override IndeterminateRelativePath Combine(string relativePath)
    {
        return new IndeterminateRelativePath(Path.Combine(LocationString, relativePath));
    }

    /// <inheritdoc/>
    public override IndeterminateRelativePath Combine(RelativeLocalPath relativeLocalPath)
    {
        return new IndeterminateRelativePath(Path.Combine(LocationString, relativeLocalPath.LocationString));
    }
}
