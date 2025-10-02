using System.Text.Json;
using System.Text.Json.Serialization;

using Brokkr.Location.Abstractions;

namespace Brokkr.Location.Converter;

/// <summary>
/// JSON converter for the <see cref="AbsoluteLocalPath"/> type.
/// </summary>
public class AbsoluteLocalPathJsonConverter : JsonConverter<AbsoluteLocalPath>
{
    /// <inheritdoc/>
    public override AbsoluteLocalPath? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrWhiteSpace(str))
        {
            return null;
        }

        if (AbsoluteWindowsPath.TryCreate(str, out var windowsPath))
        {
            return windowsPath;
        }

        if (AbsoluteUnixPath.TryCreate(str, out var unixPath))
        {
            return unixPath;
        }

        throw new JsonException($"Could not convert '{str}' to '{typeof(AbsoluteLocalPath).FullName}'.");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, AbsoluteLocalPath value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.LocationString);
    }
}