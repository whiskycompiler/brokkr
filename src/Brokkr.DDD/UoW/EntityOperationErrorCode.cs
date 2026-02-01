namespace Brokkr.DDD.UoW;

/// <summary>
/// Error codes of entity operations.
/// </summary>
public enum EntityOperationErrorCode
{
    /// <summary>
    /// Indicates that the operation failed but the cause could not be determined.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Indicates that operation failed due to a generic error probably unrelated to the entity itself.
    /// Example: permission error, network error, etc.
    /// </summary>
    GenericError = 1,

    /// <summary>
    /// Indicates that operation failed because the entity already exists.
    /// </summary>
    EntityAlreadyExists = 2,

    /// <summary>
    /// Indicates that operation failed because the entity does not exist.
    /// </summary>
    EntityDoesNotExist = 3,

    /// <summary>
    /// Inidicates that operation failed because of an error in the transaction unrelated to this entity itself.
    /// </summary>
    FailedBecauseOfTransaction = 4,
}
