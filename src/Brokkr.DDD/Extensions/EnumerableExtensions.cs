using Brokkr.DDD.ChangeTracking;

namespace Brokkr.DDD.Extensions;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Creates a deep clone of each element in a sequence of <see cref="IDeepCloneable{T}"/>s.
    /// </summary>
    public static IEnumerable<T> DeepSequenceCopy<T>(this IEnumerable<IDeepCloneable<T>> enumerable)
    {
        return enumerable.Select(s => s.DeepClone());
    }
}
