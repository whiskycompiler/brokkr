using System.Text.Json.Serialization;

using Brokkr.Location.Converter;

namespace Brokkr.Location.Abstractions;

/// <summary>
/// Base record for absolute local paths.
/// </summary>
[JsonConverter(typeof(LocationJsonConverterFactory))]
public abstract record AbsoluteLocalPath : LocalPath
{
}