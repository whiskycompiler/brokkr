using System.Globalization;
using System.Numerics;

namespace Brokkr.Core.Extensions;

/// <summary>
/// Extensions for <see cref="INumber{TSelf}"/> and concrete number types.
/// </summary>
public static class NumberExtensions
{
    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant<T>(this INumber<T> number) where T : INumber<T>?
    {
        return number.ToString(null, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this int number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this uint number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }
    
    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this nint number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }
    
    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this nuint number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }
    
    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this byte number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this sbyte number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this short number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this ushort number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this long number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this ulong number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this float number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this double number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the number to its equivalent string representation using the invariant culture.
    /// </summary>
    public static string ToStringInvariant(this decimal number)
    {
        return number.ToString(CultureInfo.InvariantCulture);
    }
}
