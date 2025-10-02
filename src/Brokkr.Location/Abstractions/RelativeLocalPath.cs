using System.Text.Json.Serialization;

using Brokkr.Location.Converter;

namespace Brokkr.Location.Abstractions;

/// <summary>
/// Base record for relative local paths.
/// </summary>
[JsonConverter(typeof(LocationJsonConverterFactory))]
public abstract record RelativeLocalPath : LocalPath
{
    
}
