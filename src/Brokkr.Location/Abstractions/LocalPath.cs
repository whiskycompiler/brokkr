using System.Text.Json.Serialization;

using Brokkr.Location.Converter;

namespace Brokkr.Location.Abstractions;

/// <summary>
/// Base record for local paths.
/// </summary>
[JsonConverter(typeof(LocationJsonConverterFactory))]
public abstract record LocalPath : LocationBase
{
    /// <summary>
    /// Combines the path with another relative path.
    /// </summary>
    public abstract LocalPath Combine(string relativePath);
    
    /// <summary>
    /// Combines the path with another relative path.
    /// </summary>
    public abstract LocalPath Combine(RelativeLocalPath relativeLocalPath);
}
