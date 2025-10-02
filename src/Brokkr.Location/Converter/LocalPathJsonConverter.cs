using System.Text.Json;
using System.Text.Json.Serialization;

using Brokkr.Location.Abstractions;

namespace Brokkr.Location.Converter;

/// <summary>
/// JSON converter for the <see cref="LocalPath"/> type.
/// </summary>
public class LocalPathJsonConverter : JsonConverter<LocalPath>
{
    /// <inheritdoc/>
    public override LocalPath? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrWhiteSpace(str))
        {
            return null;
        }

        if (Location.TryCreate(str, out var location) && location is LocalPath path)
        {
            return path;
        }

        throw new JsonException($"Could not convert '{str}' to '{typeof(LocalPath).FullName}'.");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, LocalPath value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.LocationString);
    }
}