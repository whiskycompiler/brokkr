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
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="enumerable">The sequence of <see cref="IDeepCloneable{T}"/>s to clone.</param>
    /// <returns>A new sequence of cloned elements.</returns>
    public static IEnumerable<T> DeepSequenceCopy<T>(this IEnumerable<IDeepCloneable<T>> enumerable)
    {
        return enumerable.Select(s => s.DeepClone());
    }
}
