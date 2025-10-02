using System.Text.Json;
using System.Text.Json.Serialization;

using Brokkr.Location.Abstractions;

namespace Brokkr.Location.Converter;

/// <summary>
/// Generic JSON converter for concrete location types.
/// </summary>
/// <typeparam name="TType">The type of location to convert.</typeparam>
public class ConcreteLocationJsonConverter<TType> : JsonConverter<TType>
    where TType : ILocation, IStaticLocationFactory<TType>
{
    /// <inheritdoc/>
    public override TType? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrWhiteSpace(str))
        {
            return default;
        }

        if (TType.TryCreate(str, out var location))
        {
            return location;
        }

        throw new JsonException($"Could not convert '{str}' to '{typeof(TType).FullName}'.");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.LocationString);
    }
}