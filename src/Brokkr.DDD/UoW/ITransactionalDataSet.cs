using Brokkr.DDD.ChangeTracking;

namespace Brokkr.DDD.UoW;

/// <summary>
/// Describes a data set that behaves transactional when saving changes.
/// </summary>
public interface ITransactionalDataSet
{
    /// <summary>
    /// Adds an entity change to the current transaction.
    /// </summary>
    /// <param name="entry">The tracker entry representing the entity change to add to the transaction.</param>
    void AddEntityChangeToTransaction(ITrackerEntry entry);

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <returns>A task that represents the asynchronous begin transaction operation.</returns>
    Task BeginTransaction();

    /// <summary>
    /// Commits the current transaction by applying all pending entity changes.
    /// </summary>
    /// <returns>A list of operation failures that occurred during the commit.</returns>
    Task<List<EntityOperationFailure>> CommitTransaction();
}