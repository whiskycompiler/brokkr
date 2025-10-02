using System.Text.Json;
using System.Text.Json.Serialization;

namespace Brokkr.Location.Converter;

/// <summary>
/// Factory for creating JSON converters for location types.
/// </summary>
public class LocationJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        return Location.JsonConverters.ContainsKey(typeToConvert);
    }

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return Location.JsonConverters.GetValueOrDefault(typeToConvert);
    }
}