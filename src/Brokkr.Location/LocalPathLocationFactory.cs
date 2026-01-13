using System.Diagnostics.CodeAnalysis;

using Brokkr.Location.Abstractions;

namespace Brokkr.Location;

/// <summary>
/// Location factory supporting only local paths.
/// </summary>
public class LocalPathLocationFactory : ILocationFactory
{
    /// <inheritdoc/>
    public ILocation Create(string locationString)
    {
        if (TryCreate(locationString, out var location))
        {
            return location;
        }

        throw new ArgumentException("Location string could not be parsed.", nameof(locationString));
    }

    /// <inheritdoc/>
    public bool TryCreate(string locationString, [NotNullWhen(true)] out ILocation? location)
    {
        if (string.IsNullOrWhiteSpace(locationString))
        {
            throw new ArgumentException("Location string cannot be null or whitespace", nameof(locationString));
        }

        //
        // Starting here are obvious locations which should not cause any differentiation problems
        //

        if (AbsoluteWindowsPath.TryCreate(locationString, out var absoluteWindowsPath))
        {
            location = absoluteWindowsPath;
            return true;
        }

        if (AbsoluteUnixPath.TryCreate(locationString, out var absoluteUnixPath))
        {
            location = absoluteUnixPath;
            return true;
        }

        //
        // Starting here picking needs to be ordered to prevent type confusion
        //

        // this is for relative paths where the OS is not determinable (e.g. just a file name)
        if (IndeterminateRelativePath.TryCreate(locationString, out var indeterminateRelativePath))
        {
            location = indeterminateRelativePath;
            return true;
        }

        // needs to be after indeterminate relative path check
        if (RelativeWindowsPath.TryCreate(locationString, out var relativeWindowsPath))
        {
            location = relativeWindowsPath;
            return true;
        }

        // needs to be after indeterminate relative path check
        if (RelativeUnixPath.TryCreate(locationString, out var relativeUnixPath))
        {
            location = relativeUnixPath;
            return true;
        }

        location = null;
        return false;
    }
}
