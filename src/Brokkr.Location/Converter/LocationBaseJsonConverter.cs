using System.Text.Json;
using System.Text.Json.Serialization;

using Brokkr.Location.Abstractions;

namespace Brokkr.Location.Converter;

/// <summary>
/// JSON converter for the <see cref="LocationBase"/> type.
/// </summary>
public class LocationBaseJsonConverter : JsonConverter<LocationBase>
{
    /// <inheritdoc/>
    public override LocationBase? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrWhiteSpace(str))
        {
            return null;
        }

        if (Location.TryCreate(str, out var location))
        {
            return location as LocationBase;
        }

        throw new JsonException($"Could not convert '{str}' to '{typeof(LocationBase).FullName}'.");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, LocationBase value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.LocationString);
    }
}
