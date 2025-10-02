namespace Brokkr.DDD.UoW;

/// <summary>
/// Describes a repository that tracks changes to entities in order to combine mutliple operations into transactions.
/// Saving the changes is done via the related <see cref="IUnitOfWork"/>.
/// </summary>
public interface IUnitOfWorkPatternRepository
{
}

/// <summary>
/// Describes a repository that tracks changes to entities in order to combine mutliple operations into transactions.
/// Saving the changes is done via the related <see cref="IUnitOfWork"/>.
/// </summary>
public interface IUnitOfWorkPatternRepository<in TEntity> : IUnitOfWorkPatternRepository
{
    /// <summary>
    /// Adds a new entity to the tracker.
    /// </summary>
    void AddEntity(TEntity entity);

    /// <summary>
    /// Adds an existing entity to the tracker flagged as modified.
    /// Saving changes will fail if this entity does not exist in the storage.
    /// </summary>
    void UpdateEntity(TEntity entity);

    /// <summary>
    /// Adds an existing entity to the tracker flagged for deletion.
    /// Saving changes will not fail if this entity does not exist in the storage.
    /// </summary>
    void RemoveEntity(TEntity entity);
}
