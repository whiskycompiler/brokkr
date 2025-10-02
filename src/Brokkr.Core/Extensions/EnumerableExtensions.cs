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
}
