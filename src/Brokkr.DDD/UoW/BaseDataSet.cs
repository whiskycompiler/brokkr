using Brokkr.DDD.ChangeTracking;

namespace Brokkr.DDD.UoW;

/// <summary>
/// Basic implementation of a data set used for entity operations.
/// </summary>
public abstract class BaseDataSet<TEntity> : IDataSet<TEntity> where TEntity : class, ITrackable<TEntity>
{
    /// <summary>
    /// Change tracker to use for the data set.
    /// </summary>
    protected ChangeTracker ChangeTracker { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDataSet{TEntity}"/> class.
    /// </summary>
    protected BaseDataSet(ChangeTracker changeTracker)
    {
        ChangeTracker = changeTracker;
    }

    /// <inheritdoc/>
    public void AddEntity(TEntity entity)
    {
        ChangeTracker.AddOrUpdateEntry(entity, TrackingState.Added, this);
    }

    /// <inheritdoc/>
    public void UpdateEntity(TEntity entity)
    {
        ChangeTracker.AddOrUpdateEntry(entity, TrackingState.Modified, this);
    }

    /// <inheritdoc/>
    public void DeleteEntity(TEntity entity)
    {
        ChangeTracker.AddOrUpdateEntry(entity, TrackingState.Deleted, this);
    }

    /// <inheritdoc/>
    public abstract Func<Task<EntityOperationFailure?>> GetOperationForEntityChange(ITrackerEntry entry);
}
