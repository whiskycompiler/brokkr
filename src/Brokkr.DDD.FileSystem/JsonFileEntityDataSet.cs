using System.Text.Json;

using Brokkr.Core.Common;
using Brokkr.DDD.ChangeTracking;
using Brokkr.DDD.FileSystem.Abstractions;
using Brokkr.DDD.UoW;
using Brokkr.Location.Abstractions;

using Microsoft.Extensions.Logging;

namespace Brokkr.DDD.FileSystem;

/// <summary>
/// Represents a data set that consists of multiple entities each in its own JSON file.
/// </summary>
public sealed partial class JsonFileEntityDataSet<TEntity, TId> : BaseFileDataSet<TEntity>
    where TEntity : class, ITrackable<TEntity>, IEntity<TId>
    where TId : notnull
{
    private readonly LocalPath? _folderPath;
    private readonly ILogger _logger;
    private readonly IEntityPathResolver<TId>? _entityPathResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileEntityDataSet{TEntity,TId}"/> class.
    /// </summary>
    /// <param name="changeTracker">The change tracker used for entities in this set.</param>
    /// <param name="folderPath">The path to the foldeer containing the JSON files.</param>
    /// <param name="logger">Logger to log things.</param>
    /// <param name="allowUpsert"><see cref="BaseFileDataSet{TEntity}.AllowUpsert"/></param>
    /// <param name="ignoreNonexistentFilesWhenDeleting"><see cref="BaseFileDataSet{TEntity}.IgnoreNonexistentEntitiesWhenDeleting"/></param>
    public JsonFileEntityDataSet(
        ChangeTracker changeTracker,
        LocalPath folderPath,
        ILogger logger,
        bool allowUpsert = true,
        bool ignoreNonexistentFilesWhenDeleting = true)
        : base(changeTracker, allowUpsert, ignoreNonexistentFilesWhenDeleting)
    {
        _folderPath = folderPath;
        _logger = logger;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileEntityDataSet{TEntity,TId}"/> class.
    /// </summary>
    /// <param name="changeTracker">The change tracker used for entities in this set.</param>
    /// <param name="entityPathResolver">Component to resolve the entities physical location.</param>
    /// <param name="logger">Logger to log things.</param>
    /// <param name="allowUpsert"><see cref="BaseFileDataSet{TEntity}.AllowUpsert"/></param>
    /// <param name="ignoreNonexistentFilesWhenDeleting"><see cref="BaseFileDataSet{TEntity}.IgnoreNonexistentEntitiesWhenDeleting"/></param>
    public JsonFileEntityDataSet(
        ChangeTracker changeTracker,
        IEntityPathResolver<TId> entityPathResolver,
        ILogger logger,
        bool allowUpsert = true,
        bool ignoreNonexistentFilesWhenDeleting = true)
        : base(changeTracker, allowUpsert, ignoreNonexistentFilesWhenDeleting)
    {
        _entityPathResolver = entityPathResolver;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override Func<Task<EntityOperationFailure?>> GetOperationForEntityChange(ITrackerEntry entry)
    {
        return entry.State switch
        {
            // usually you should not even call this method for those states
            TrackingState.Detached or TrackingState.Unchanged =>
                () => Task.FromResult<EntityOperationFailure?>(null),

            TrackingState.Added =>
                async () =>
                {
                    var entityPath = await GetEntityPath(entry);
                    return await AddNewFile(entry, entityPath, JsonSerializer.Serialize(entry.TrackedInstance));
                },

            TrackingState.Modified =>
                async () =>
                {
                    var entityPath = await GetEntityPath(entry);
                    return await UpdateFile(entry, entityPath, JsonSerializer.Serialize(entry.TrackedInstance));
                },

            TrackingState.Deleted =>
                async () =>
                {
                    var entityPath = await GetEntityPath(entry);
                    return await DeleteFile(entry, entityPath);
                },

            _ => throw new UnhandleableEnumValueException(entry.State),
        };
    }

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="track">Specifies whether the entity should be tracked for changes. Defaults to false.</param>
    /// <returns> The entity if found; otherwise, null.</returns>
    public async Task<TEntity?> GetById(TId id, bool track = false)
    {
        var path = await GetEntityPath(id);
        return await GetEntityFromFile(path, track);
    }

    /// <summary>
    /// Retrieves an entity by its location.
    /// </summary>
    /// <param name="path">The path of the entity.</param>
    /// <param name="track">Specifies whether the entity should be tracked for changes. Defaults to false.</param>
    /// <returns> The entity if found; otherwise, null.</returns>
    public async Task<TEntity?> GetByLocation(LocalPath path, bool track = false)
    {
        return await GetEntityFromFile(path, track);
    }

    /// <summary>
    /// Asynchronously enumerates all entities in the data set.
    /// </summary>
    /// <returns>An asynchronous enumerable of entities of type <typeparamref name="TEntity"/>.</returns>
    public async IAsyncEnumerable<TEntity> EnumerateAsync()
    {
        if (_entityPathResolver is IAsyncEntityPathResolver<TId> asyncResolver)
        {
            await foreach (var path in asyncResolver.GetAllPathsAsync())
            {
                var entity = await GetEntityFromFile(path, false);
                if (entity == null)
                {
                    LogEntityAtExpectedPathCouldNotBeFetched(path);
                    continue;
                }

                yield return entity;
            }

            yield break;
        }

        var paths = _folderPath is not null
            ? Directory.EnumerateFiles(_folderPath.LocationString, "*.json")
            : _entityPathResolver!.GetAllPaths(); // suppressed because resolver is not null when _folderPath is null

        foreach (var path in paths)
        {
            var entity = await GetEntityFromFile(path, false);
            if (entity == null)
            {
                LogEntityAtExpectedPathCouldNotBeFetched(path);
                continue;
            }

            yield return entity;
        }
    }

    private ValueTask<LocalPath> GetEntityPath(ITrackerEntry entry)
    {
        // suppress cast issues because the repo should never get an entry not belonging to it
        var entity = (entry.TrackedInstance as IEntity<TId>)!;
        return GetEntityPath(entity.Id);
    }

    private async ValueTask<LocalPath> GetEntityPath(TId id)
    {
        return _entityPathResolver switch
        {
            null => _folderPath!.Combine($"{id}.json"),
            IAsyncEntityPathResolver<TId> asyncResolver => LocalPath.Create(await asyncResolver.GetPathFromIdAsync(id)),
            _ => LocalPath.Create(_entityPathResolver.GetPathFromId(id)),
        };
    }

    [LoggerMessage(LogLevel.Warning, "Entity at expected path {Path} could not be fetched.")]
    partial void LogEntityAtExpectedPathCouldNotBeFetched(string path);
}
