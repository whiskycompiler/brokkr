using System.Text.Json;
using System.Text.Json.Serialization;

using Brokkr.Location.Abstractions;

namespace Brokkr.Location.Converter;

/// <summary>
/// JSON converter for the <see cref="RelativeLocalPath"/> type.
/// </summary>
public class RelativeLocalPathJsonConverter : JsonConverter<RelativeLocalPath>
{
    /// <inheritdoc/>
    public override RelativeLocalPath? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrWhiteSpace(str))
        {
            return null;
        }

        if (IndeterminateRelativePath.TryCreate(str, out var indeterminateRelativePath))
        {
            return indeterminateRelativePath;
        }

        if (RelativeWindowsPath.TryCreate(str, out var windowsPath))
        {
            return windowsPath;
        }

        if (RelativeUnixPath.TryCreate(str, out var unixPath))
        {
            return unixPath;
        }

        throw new JsonException($"Could not convert '{str}' to '{typeof(RelativeLocalPath).FullName}'.");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RelativeLocalPath value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.LocationString);
    }
}
