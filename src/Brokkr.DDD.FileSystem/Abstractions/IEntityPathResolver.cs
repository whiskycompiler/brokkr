namespace Brokkr.DDD.FileSystem.Abstractions;

/// <summary>
/// Provides functionality to resolve paths for entities based on their identifiers.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public interface IEntityPathResolver<in TId>
{
    /// <summary>
    /// Gets the path for an entity based on its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>The path for the entity.</returns>
    string GetPathFromId(TId id);

    /// <summary>
    /// Gets the paths for all entities.
    /// </summary>
    /// <returns>A collection of all entity paths.</returns>
    IEnumerable<string> GetAllPaths();
}
