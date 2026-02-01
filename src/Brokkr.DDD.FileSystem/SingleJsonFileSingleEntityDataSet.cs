using System.Text.Json;

using Brokkr.Core.Common;
using Brokkr.DDD.ChangeTracking;
using Brokkr.DDD.UoW;
using Brokkr.Location.Abstractions;

namespace Brokkr.DDD.FileSystem;

/// <summary>
/// Represents a data set that consists of a single entity in a JSON file.
/// </summary>
public sealed class SingleJsonFileSingleEntityDataSet<TEntity> : BaseFileDataSet<TEntity>
    where TEntity : class, ITrackable<TEntity>
{
    private readonly LocalPath _jsonFilePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleJsonFileSingleEntityDataSet{TEntity}"/> class.
    /// </summary>
    /// <param name="changeTracker">The change tracker used for entities in this set.</param>
    /// <param name="jsonFilePath">The path to the JSON file.</param>
    /// <param name="allowUpsert"><see cref="BaseFileDataSet{TEntity}.AllowUpsert"/></param>
    /// <param name="ignoreNonexistentFilesWhenDeleting"><see cref="BaseFileDataSet{TEntity}.IgnoreNonexistentEntitiesWhenDeleting"/></param>
    public SingleJsonFileSingleEntityDataSet(
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
        return entry.State switch
        {
            // usually you should not even call this method for those states
            TrackingState.Detached or TrackingState.Unchanged =>
                () => Task.FromResult<EntityOperationFailure?>(null),

            TrackingState.Added =>
                () => AddNewFile(entry, _jsonFilePath, JsonSerializer.Serialize(entry.TrackedInstance)),

            TrackingState.Modified =>
                () => UpdateFile(entry, _jsonFilePath, JsonSerializer.Serialize(entry.TrackedInstance)),

            TrackingState.Deleted =>
                () => DeleteFile(entry, _jsonFilePath),

            _ => throw new UnhandleableEnumValueException(entry.State),
        };
    }

    /// <summary>
    /// Retrieves the entity from the JSON file.
    /// </summary>
    /// <param name="track">Flag to indicate if the entity should be tracked.</param>
    /// <returns>The entity.</returns>
    public async Task<TEntity?> GetEntity(bool track = false)
    {
        return await GetEntityFromFile(_jsonFilePath, track);
    }
}
