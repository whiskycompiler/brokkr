using System.Text.Json;
using System.Text.Json.Serialization;

namespace Brokkr.Location.Converter;

/// <summary>
/// JSON converter for the <see cref="RelativeUnixPath"/> type.
/// </summary>
/// <remarks>
/// Disallows backslashes so that windows paths are rejected.
/// </remarks>
public class RelativeUnixPathJsonConverter : JsonConverter<RelativeUnixPath>
{
    /// <inheritdoc/>
    public override RelativeUnixPath? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrWhiteSpace(str))
        {
            return null;
        }

        if (RelativeUnixPath.TryCreate(str, out var unixPath, disallowBackslash: true))
        {
            return unixPath;
        }

        throw new JsonException($"Could not convert '{str}' to '{typeof(RelativeUnixPath).FullName}'.");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RelativeUnixPath value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.LocationString);
    }
}