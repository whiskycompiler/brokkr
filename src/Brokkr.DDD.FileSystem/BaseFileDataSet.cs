using System.Text.Json;

using Brokkr.DDD.ChangeTracking;
using Brokkr.DDD.UoW;
using Brokkr.Location.Abstractions;

namespace Brokkr.DDD.FileSystem;

/// <summary>
/// Provides a base implementation for data sets that are using local files.
/// </summary>
public abstract class BaseFileDataSet<TEntity> : BaseDataSet<TEntity> where TEntity : class, ITrackable<TEntity>
{
    /// <summary>
    /// Bool indicating whether upsert operations are allowed for files that do not exist.
    /// </summary>
    public bool AllowUpsert { get; }

    /// <summary>
    /// Bool indicating whether to ignore nonexistent entities/files during deletion operations.
    /// </summary>
    public bool IgnoreNonexistentEntitiesWhenDeleting { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFileDataSet{TEntity}"/> class.
    /// </summary>
    protected BaseFileDataSet(
        ChangeTracker tracker,
        bool allowUpsert = true,
        bool ignoreNonexistentFilesWhenDeleting = true)
        : base(tracker)
    {
        AllowUpsert = allowUpsert;
        IgnoreNonexistentEntitiesWhenDeleting = ignoreNonexistentFilesWhenDeleting;
    }

    /// <summary>
    /// Executes an entity addition operation by creating a new file with the specified content.
    /// </summary>
    /// <param name="entry">The tracker entry of the new entity.</param>
    /// <param name="filePath">The path where the new file will be created.</param>
    /// <param name="fileContent">The content to write to the new file.</param>
    /// <returns>A task that completes with an <see cref="EntityOperationFailure"/> if the operation fails, or <see langword="null"/> if it succeeds.</returns>
    protected static Task<EntityOperationFailure?> AddNewFile(
        ITrackerEntry entry,
        LocalPath filePath,
        string fileContent)
    {
        if (File.Exists(filePath))
        {
            return Task.FromResult<EntityOperationFailure?>(
                new EntityOperationFailure(
                    entry.TrackedInstance,
                    EntityOperationErrorCode.EntityAlreadyExists));
        }

        EnsureFileDirectoryExists(filePath);
        File.WriteAllText(filePath, fileContent);

        return Task.FromResult<EntityOperationFailure?>(null);
    }

    /// <summary>
    /// Executes an entity update operation by modifying the existing file of the entity with the specified content.
    /// </summary>
    /// <param name="entry">The tracker entry of the updated entity.</param>
    /// <param name="filePath">The path of the file belonging to the entity.</param>
    /// <param name="fileContent">The new content to write to the file.</param>
    /// <returns>A task that completes with an <see cref="EntityOperationFailure"/> if the operation fails, or <see langword="null"/> if it succeeds.</returns>
    protected Task<EntityOperationFailure?> UpdateFile(ITrackerEntry entry, LocalPath filePath, string fileContent)
    {
        if (!File.Exists(filePath))
        {
            if (!AllowUpsert)
            {
                return Task.FromResult<EntityOperationFailure?>(
                    new EntityOperationFailure(
                        entry.TrackedInstance,
                        EntityOperationErrorCode.EntityDoesNotExist));
            }

            EnsureFileDirectoryExists(filePath);
        }

        File.WriteAllText(filePath, fileContent);

        return Task.FromResult<EntityOperationFailure?>(null);
    }

    /// <summary>
    /// Executes an entity deletion operation by deleting the local file associated with the entity.
    /// </summary>
    /// <param name="entry">The tracker entry associated with the file being deleted.</param>
    /// <param name="filePath">The path of the local file to be deleted.</param>
    /// <returns>A task that completes with an <see cref="EntityOperationFailure"/> if the operation fails, or <see langword="null"/> if it succeeds.</returns>
    protected Task<EntityOperationFailure?> DeleteFile(ITrackerEntry entry, LocalPath filePath)
    {
        if (!File.Exists(filePath) && !IgnoreNonexistentEntitiesWhenDeleting)
        {
            return Task.FromResult<EntityOperationFailure?>(
                new EntityOperationFailure(
                    entry.TrackedInstance,
                    EntityOperationErrorCode.EntityDoesNotExist));
        }

        File.Delete(filePath);
        return Task.FromResult<EntityOperationFailure?>(null);
    }

    /// <summary>
    /// Retrieves an entity of type <typeparamref name="TEntity"/> from a file at the specified path. Optionally tracks the entity.
    /// </summary>
    /// <param name="filePath">The path to the file containing the entity data.</param>
    /// <param name="track">Indicates whether or not to track the retrieved entity.</param>
    /// <returns>The entity of type <typeparamref name="TEntity"/> if existing, otherwise null.</returns>
    /// <exception cref="EntityOperationException">Thrown if an error occurs while reading or deserializing the entity from the file.</exception>
    protected TEntity? GetEntityFromFile(LocalPath filePath, bool track)
    {
        if (File.Exists(filePath))
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var entity = JsonSerializer.Deserialize<TEntity>(json);
                if (entity != null && track)
                {
                    ChangeTracker.AddOrUpdateEntry(entity, TrackingState.Unchanged, this);
                }

                return entity;
            }
            catch (Exception e)
            {
                throw new EntityOperationException([], e, "Failed to read and deserialize entity from file.");
            }
        }

        return null;
    }

    /// <summary>
    /// Ensures that the directory for the specified file path exists.
    /// If the directory does not exist, it is created.
    /// </summary>
    /// <param name="filePath">The file path for which to ensure the directory exists.</param>
    /// <remarks>
    /// If a directory is specified as the path, the containing directory is created not the specified directory itself.
    /// </remarks>
    protected static void EnsureFileDirectoryExists(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (directory != null)
        {
            Directory.CreateDirectory(directory);
        }
    }
}
