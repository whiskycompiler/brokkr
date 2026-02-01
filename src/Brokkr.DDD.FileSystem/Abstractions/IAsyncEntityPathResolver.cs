namespace Brokkr.DDD.FileSystem.Abstractions;

/// <summary>
/// Provides asynchronous functionality to resolve paths for entities based on their identifiers.
/// Extends <see cref="IEntityPathResolver{TId}"/> with async support.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public interface IAsyncEntityPathResolver<in TId> : IEntityPathResolver<TId>
{
    /// <summary>
    /// Asynchronously gets the path for an entity based on its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>The path for the entity.</returns>
    Task<string> GetPathFromIdAsync(TId id);

    /// <summary>
    /// Asyncronously gets the paths for all entities.
    /// </summary>
    /// <returns>A collection of all entity paths.</returns>
    IAsyncEnumerable<string> GetAllPathsAsync();
}
