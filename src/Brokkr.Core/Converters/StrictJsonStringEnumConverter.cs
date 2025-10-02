using System.Text.Json.Serialization;

namespace Brokkr.Core.Converters;

/// <summary>
/// A <see cref="JsonStringEnumConverter{TEnum}"/> that ONLY allows string values.
/// </summary>
/// <typeparam name="TEnum">The enum type.</typeparam>
/// <seealso cref="JsonStringEnumConverter{TEnum}" />
public class StrictJsonStringEnumConverter<TEnum> : JsonStringEnumConverter<TEnum> where TEnum : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrictJsonStringEnumConverter{TEnum}"/> class.
    /// </summary>
    public StrictJsonStringEnumConverter() : base(allowIntegerValues: false)
    {
        
    }
}
