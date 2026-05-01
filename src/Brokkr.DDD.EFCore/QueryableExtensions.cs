using Microsoft.EntityFrameworkCore;

namespace Brokkr.DDD.EFCore;

/// <summary>
/// Extension methods for <see cref="IQueryable{T}"/>
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Configures the query to track or not track the changes of entity objects during execution.
    /// </summary>
    /// <typeparam name="TResult">The type of the entities in the query.</typeparam>
    /// <param name="queryable">The source queryable object.</param>
    /// <param name="shouldTrack">A boolean value indicating whether the query should track entity objects.</param>
    /// <returns>An <see cref="IQueryable{T}"/> that is configured based on the tracking preference.</returns>
    public static IQueryable<TResult> TrackEntity<TResult>(this IQueryable<TResult> queryable, bool shouldTrack)
        where TResult : class
    {
        return shouldTrack ? queryable : queryable.AsNoTracking();
    }

    /// <summary>
    /// Returns a subset of elements from the source queryable object, based on the specified page number and page size.
    /// </summary>
    /// <typeparam name="TResult">The type of the entities in the query.</typeparam>
    /// <param name="queryable">The source queryable object.</param>
    /// <param name="page">The page number to retrieve. The first page is 1.</param>
    /// <param name="pageSize">The number of items to include in each page.</param>
    /// <returns>An <see cref="IQueryable{T}"/> containing the elements for the specified page.</returns>
    public static IQueryable<TResult> GetPage<TResult>(
        this IQueryable<TResult> queryable,
        int page,
        int pageSize)
    {
        return queryable
            .Skip((Math.Max(page, 1) - 1) * pageSize)
            .Take(pageSize);
    }
}
