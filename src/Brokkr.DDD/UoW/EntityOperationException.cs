namespace Brokkr.DDD.UoW;

/// <summary>
/// Exception for failed entity operations in <see cref="IUnitOfWork"/>s.
/// </summary>
public class EntityOperationException : Exception
{
    /// <summary>
    /// Collection of entities that failed to complete their operations, along with associated error details.
    /// </summary>
    public IReadOnlyCollection<EntityOperationFailure> FailedEntities { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityOperationException"/> class.
    /// </summary>
    public EntityOperationException(
        IReadOnlyCollection<EntityOperationFailure> failedEntities,
        string? message = null)
        : base(message)
    {
        FailedEntities = failedEntities;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityOperationException"/> class.
    /// </summary>
    public EntityOperationException(
        IReadOnlyCollection<EntityOperationFailure> failedEntities,
        Exception innerException,
        string? message = null)
        : base(message, innerException)
    {
        FailedEntities = failedEntities;
    }
}
