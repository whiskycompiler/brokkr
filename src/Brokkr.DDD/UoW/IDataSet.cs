using Brokkr.DDD.ChangeTracking;

namespace Brokkr.DDD.UoW;

/// <summary>
/// Defines a data set that can handle entity change operations.
/// </summary>
public interface IDataSet
{
    /// <summary>
    /// Gets the operation to execute to apply the given entity change defined by the tracker entry.
    /// </summary>
    /// <param name="entry">The tracker entry containing information about the entity change.</param>
    /// <returns>A function that returns a task with an <see cref="EntityOperationFailure"/> if the operation fails, or <see langword="null"/> if it succeeds.</returns>
    /// <remarks>
    /// It is recommended to not call this method for tracker entries with states that do not require
    /// any operation to be executed (<see cref="TrackingState.Detached"/> and <see cref="TrackingState.Unchanged"/>).
    /// </remarks>
    Func<Task<EntityOperationFailure?>> GetOperationForEntityChange(ITrackerEntry entry);
}

/// <summary>
/// Describes a <see cref="IDataSet"/> set typed for a specific entity.
/// </summary>
public interface IDataSet<in TEntity> : IDataSet where TEntity : class, ITrackable<TEntity>
{
    /// <summary>
    /// Flags the entity in the change tracking as new.
    /// </summary>
    void AddEntity(TEntity entity);

    /// <summary>
    /// Flags the entity in the change tracking as updated.
    /// </summary>
    void UpdateEntity(TEntity entity);

    /// <summary>
    /// Flags the entity in the change tracking as deleted. 
    /// </summary>
    void DeleteEntity(TEntity entity);
}
