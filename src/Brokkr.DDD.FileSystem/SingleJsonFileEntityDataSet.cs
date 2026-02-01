using System.Text.Json;

using Brokkr.Core.Common;
using Brokkr.Core.Extensions;
using Brokkr.DDD.ChangeTracking;
using Brokkr.DDD.UoW;
using Brokkr.Location.Abstractions;

namespace Brokkr.DDD.FileSystem;

/// <summary>
/// Represents a data set that consists of multiple entities in a single JSON file.
/// </summary>
public sealed class SingleJsonFileEntityDataSet<TEntity, TId>
    : BaseFileDataSet<TEntity>, ITransactionalDataSet, IDisposable
    where TEntity : class, ITrackable<TEntity>, IEntity<TId>
    where TId : notnull
{
    // ReSharper disable StaticMemberInGenericType | intended - those are type specific statics
    private static Task CurrentTransactionTask = Task.CompletedTask;
    private static readonly SemaphoreSlim FileStreamCreationLock = new(1, 1);

    private readonly LocalPath _jsonFilePath;
    private FileStream? _transactionFileStream;
    private TaskCompletionSource? _transactionTaskCompletionSource;
    private readonly List<ITrackerEntry> _transactionEntries = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleJsonFileEntityDataSet{TEntity,TId}"/> class.
    /// </summary>
    /// <param name="changeTracker">The change tracker used for entities in this set.</param>
    /// <param name="jsonFilePath">The path to the JSON file.</param>
    /// <param name="allowUpsert"><see cref="BaseFileDataSet{TEntity}.AllowUpsert"/></param>
    /// <param name="ignoreNonexistentFilesWhenDeleting"><see cref="BaseFileDataSet{TEntity}.IgnoreNonexistentEntitiesWhenDeleting"/></param>
    public SingleJsonFileEntityDataSet(
        ChangeTracker changeTracker,
        LocalPath jsonFilePath,
        bool allowUpsert = true,
        bool ignoreNonexistentFilesWhenDeleting = true)
        : base(changeTracker, allowUpsert, ignoreNonexistentFilesWhenDeleting)
    {
        _jsonFilePath = jsonFilePath;
    }

    /// <inheritdoc/>
    public override Func<Task<EntityOperationFailure?>> GetOperationForEntityChange(ITrackerEntry entry)
    {
        throw new NotSupportedException(
            "This dataset works with the interface ITransactionalDataSet and does not support normal operations.");
    }

    /// <inheritdoc/>
    void ITransactionalDataSet.AddEntityChangeToTransaction(ITrackerEntry entry)
    {
        _transactionEntries.Add(entry);
    }

    /// <inheritdoc/>
    async Task ITransactionalDataSet.BeginTransaction()
    {
        try
        {
            // enter lock as next creator of a file stream ...
            await FileStreamCreationLock.WaitAsync();
            // ... then wait for the current transaction to finish if there is any
            await CurrentTransactionTask;

            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            while (true)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    throw new TimeoutException(
                        $"Timeout while trying to acquire file lock for '{_jsonFilePath.LocationString}'!");
                }

                try
                {
                    _transactionFileStream = new FileStream(
                        _jsonFilePath.LocationString,
                        FileMode.OpenOrCreate,
                        FileAccess.ReadWrite,
                        FileShare.None);

                    _transactionEntries.Clear();
                    _transactionTaskCompletionSource = new TaskCompletionSource();
#pragma warning disable S2696 // Instance members should not write to "static" fields | its a shared locking mechanism
                    CurrentTransactionTask = _transactionTaskCompletionSource.Task;
#pragma warning restore S2696

                    return;
                }
                catch (IOException)
                {
                    // file is locked by another process, wait and retry
                    await Task.Delay(100, cancellationTokenSource.Token);
                }
            }
        }
        finally
        {
            FileStreamCreationLock.Release();
        }
    }

    /// <inheritdoc/>
    async Task<List<EntityOperationFailure>> ITransactionalDataSet.CommitTransaction()
    {
        if (_transactionFileStream == null || _transactionTaskCompletionSource == null)
        {
            throw new InvalidOperationException("No transaction is currently in progress.");
        }

        var entityOperationFailures = new List<EntityOperationFailure>();
        if (_transactionEntries.Count != 0)
        {
            var entityOperationSuccesses = new List<TEntity>();
            Dictionary<TId, TEntity>? fileData;
            if (_transactionFileStream.Length == 0)
            {
                fileData = new Dictionary<TId, TEntity>();
            }
            else
            {
                fileData = new Dictionary<TId, TEntity>(await JsonSerializer
                    .DeserializeAsyncEnumerable<KeyValuePair<TId, TEntity>>(_transactionFileStream)
                    .ToArrayAsync());
            }

            foreach (var transactionEntry in _transactionEntries)
            {
                var entity = (TEntity)transactionEntry.TrackedInstance;
                switch (transactionEntry.State)
                {
                    case TrackingState.Detached:
                    case TrackingState.Unchanged:
                        continue;

                    case TrackingState.Added:
                    {
                        if (!fileData.TryAdd(entity.Id, entity))
                        {
                            entityOperationFailures.Add(new EntityOperationFailure(
                                entity,
                                EntityOperationErrorCode.EntityAlreadyExists,
                                $"Entity with ID '{entity.Id}' already exists."));

                            continue;
                        }

                        break;
                    }

                    case TrackingState.Modified:
                    {
                        if (!AllowUpsert && !fileData.ContainsKey(entity.Id))
                        {
                            entityOperationFailures.Add(new EntityOperationFailure(
                                entity,
                                EntityOperationErrorCode.EntityDoesNotExist,
                                $"Entity with ID '{entity.Id}' does not exist."));

                            continue;
                        }

                        fileData[entity.Id] = entity;

                        break;
                    }

                    case TrackingState.Deleted:
                    {
                        if (!fileData.Remove(entity.Id, out _) && !IgnoreNonexistentEntitiesWhenDeleting)
                        {
                            entityOperationFailures.Add(new EntityOperationFailure(
                                entity,
                                EntityOperationErrorCode.EntityDoesNotExist,
                                $"Entity with ID '{entity.Id}' does not exist."));

                            continue;
                        }

                        break;
                    }
                    default:
                        throw new UnhandleableEnumValueException(transactionEntry.State);
                }

                entityOperationSuccesses.Add(entity);
            }

            if (entityOperationFailures.Count > 0)
            {
                entityOperationFailures.AddRange(entityOperationSuccesses
                    .Select(entity => new EntityOperationFailure(
                        entity,
                        EntityOperationErrorCode.FailedBecauseOfTransaction)));
            }
            else
            {
                _transactionFileStream.Seek(0, SeekOrigin.Begin);
                _transactionFileStream.SetLength(0);
                await JsonSerializer.SerializeAsync<IEnumerable<KeyValuePair<TId, TEntity>>>(
                    _transactionFileStream,
                    fileData);
            }
        }

        await _transactionFileStream.FlushAsync();
        await _transactionFileStream.DisposeAsync();
        _transactionFileStream = null;

        _transactionTaskCompletionSource.TrySetResult();
        _transactionTaskCompletionSource = null;

        return entityOperationFailures;
    }

    /// <summary>
    /// Retrieves an entity by its unique identifier from the JSON file.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <param name="track">Flag indicating whether to track the entity's state after retrieval.</param>
    /// <returns>The entity associated with the specified identifier, or <c>null</c> if no entity is found.</returns>
    public async Task<TEntity?> GetEntityById(TId id, bool track = false)
    {
        try
        {
            // enter lock as next creator of a file stream ...
            await FileStreamCreationLock.WaitAsync();
            // ... then wait for the current transaction to finish if there is any
            await CurrentTransactionTask;

            await using var fileStream = new FileStream(
                _jsonFilePath.LocationString,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            var entity = await JsonSerializer.DeserializeAsyncEnumerable<KeyValuePair<TId, TEntity>>(fileStream)
                .FirstOrDefaultAsync(entry => entry.Key.Equals(id));

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (track && entity.Value != null)
            {
                ChangeTracker.AddOrUpdateEntry(entity.Value, TrackingState.Unchanged, this);
            }

            return entity.Value;
        }
        finally
        {
            FileStreamCreationLock.Release();
        }
    }

    /// <summary>
    /// Retrieves a collection of entities from the dataset, optionally paginated.
    /// </summary>
    /// <param name="pageSize">The number of entities to include per page. If 0, all entities will be retrieved.</param>
    /// <param name="page">The page of entities to retrieve, starting at 1. Ignored if <paramref name="pageSize"/> is 0.</param>
    /// <param name="track">Indicates whether the retrieved entities should be tracked for changes.</param>
    /// <returns>A collection of retrieved entities.</returns>
    public async Task<IReadOnlyCollection<TEntity>> GetEntities(int pageSize = 0, int page = 1, bool track = false)
    {
        try
        {
            // enter lock as next creator of a file stream ...
            await FileStreamCreationLock.WaitAsync();
            // ... then wait for the current transaction to finish if there is any
            await CurrentTransactionTask;

            await using var fileStream = new FileStream(
                _jsonFilePath.LocationString,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            if (pageSize == 0)
            {
                return await JsonSerializer.DeserializeAsyncEnumerable<KeyValuePair<TId, TEntity>>(fileStream)
                    .Select(s => s.Value)
                    .ToArrayAsync();
            }
            else
            {
                return await JsonSerializer.DeserializeAsyncEnumerable<KeyValuePair<TId, TEntity>>(fileStream)
                    .GetPage(page, pageSize)
                    .Select(s => s.Value)
                    .ToArrayAsync();
            }
        }
        finally
        {
            FileStreamCreationLock.Release();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _transactionFileStream?.Dispose();

        // we do not set it to cancelled because we don't want exceptions to be thrown
        // because this task is only used internally to block readers when writes are in progress
        _transactionTaskCompletionSource?.TrySetResult();
    }
}
