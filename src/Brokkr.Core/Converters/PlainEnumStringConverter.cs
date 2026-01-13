using System.Text.Json;
using System.Text.Json.Serialization;

namespace Brokkr.Core.Converters;

/// <summary>
/// A JSON converter to convert plain <see cref="Enum"/> values to their string representation.
/// Because it works with the plain <see cref="Enum"/> type, deserialization is not supported as the
/// specific enum type is not known to this converter.
/// Flag enums will also only work with single values, except when combinations are explicitly defined.
/// </summary>
public class PlainEnumStringConverter : JsonConverter<Enum>
{
    /// <inheritdoc/>
    public override Enum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Operation is not supported because the specified enums type cannot be known!");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Enum value, JsonSerializerOptions options)
    {
        if (Enum.IsDefined(value.GetType(), value))
        {
            writer.WriteStringValue(value.ToString());
            return;
        }

        writer.WriteStringValue((string?)null);
    }
}
