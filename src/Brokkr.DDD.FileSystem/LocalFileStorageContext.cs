using Brokkr.DDD.ChangeTracking;
using Brokkr.DDD.UoW;

namespace Brokkr.DDD.FileSystem;

/// <summary>
/// Context to handle local files in a specific location.
/// </summary>
public abstract class LocalFileStorageContext : IUnitOfWork
{
    /// <summary>
    /// Entity change tracker of the blob context.
    /// </summary>
    public ChangeTracker ChangeTracker { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalFileStorageContext"/> class.
    /// </summary>
    protected LocalFileStorageContext()
    {
        ChangeTracker = new ChangeTracker();
    }

    /// <inheritdoc/>
    public async Task SaveTrackedChanges(CancellationToken cancellationToken = default)
    {
        var failedEntities = new List<EntityOperationFailure>();
        var entriesToProcess = ChangeTracker.Entries
            .Where(e => e.State is not TrackingState.Detached and not TrackingState.Unchanged)
            .ToArray();
        
        foreach (var entryGroup in entriesToProcess.GroupBy(e => e.DataSet))
        {
            if (entryGroup.Key is ITransactionalDataSet transactionalDataSet)
            {
                await transactionalDataSet.BeginTransaction();
                foreach (var entry in entryGroup)
                {
                    transactionalDataSet.AddEntityChangeToTransaction(entry);
                }

                var errors = await transactionalDataSet.CommitTransaction();
                failedEntities.AddRange(errors);
            }
            else
            {
                foreach (var entry in entryGroup)
                {
                    var operation = entryGroup.Key.GetOperationForEntityChange(entry);
                    var error = await operation();
                    if (error != null)
                    {
                        failedEntities.Add(error);
                    }
                }
            }
        }
        
        if (failedEntities.Count != 0)
        {
            ChangeTracker.AcceptChanges();
            return;
        }

        // Accept changes for all entities that were successfully processed
        ChangeTracker.AcceptChanges(entriesToProcess
            .Select(entry => entry.TrackedInstance)
            .Except(failedEntities.Select(s => s.Entity), ReferenceEqualityComparer.Instance));

        throw new EntityOperationException(failedEntities);
    }
}
