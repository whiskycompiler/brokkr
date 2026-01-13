using Brokkr.Core.Validation;

namespace Brokkr.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Retrieves a specific page of data based on the given page number and page size.
    /// </summary>
    /// <param name="enumerable">The collection from which to retrieve the data.</param>
    /// <param name="page">The number of the page to retrieve (1-based).</param>
    /// <param name="pageSize">The count of items in a page.</param>
    /// <returns>A the requested page of data.</returns>
    public static IEnumerable<TResult> GetPage<TResult>(
        this IEnumerable<TResult> enumerable,
        int page,
        int pageSize)
    {
        return enumerable
            .Skip((Math.Max(page, 1) - 1) * pageSize)
            .Take(pageSize);
    }

    /// <summary>
    /// Retrieves a specific page of data based on the given page number and page size.
    /// </summary>
    /// <param name="enumerable">The collection from which to retrieve the data.</param>
    /// <param name="page">The number of the page to retrieve (1-based).</param>
    /// <param name="pageSize">The count of items in a page.</param>
    /// <returns>A the requested page of data.</returns>
    public static IAsyncEnumerable<TResult> GetPage<TResult>(
        this IAsyncEnumerable<TResult> enumerable,
        int page,
        int pageSize)
    {
        return enumerable
            .Skip((Math.Max(page, 1) - 1) * pageSize)
            .Take(pageSize);
    }

    /// <summary>
    /// Throws an <see cref="ValidationException"/> if the enumeration contains any <see cref="ValidationError"/>s.
    /// </summary>
    public static void ThrowIfValidationErrorsOccurred(
        this IEnumerable<ValidationError> errors,
        string? message = null)
    {
        var collection = errors.AsIReadOnlyCollection();
        if (collection.Count > 0)
        {
            throw new ValidationException(collection, message);
        }
    }

    /// <summary>
    /// Converts the given enumerable to an <see cref="IReadOnlyCollection{T}"/>.
    /// If the enumerable is already an <see cref="IReadOnlyCollection{T}"/>, it is returned as-is.
    /// Otherwise, the enumerable is converted to an array and returned.
    /// </summary>
    /// <param name="enumerable">The source enumerable to convert.</param>
    /// <typeparam name="TEntry">The type of elements in the collection.</typeparam>
    /// <returns>An <see cref="IReadOnlyCollection{T}"/> containing the elements of the source enumerable.</returns>
    public static IReadOnlyCollection<TEntry> AsIReadOnlyCollection<TEntry>(this IEnumerable<TEntry> enumerable)
    {
        if (enumerable is IReadOnlyCollection<TEntry> result)
        {
            return result;
        }

        return enumerable.ToArray();
    }
}
