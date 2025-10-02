namespace Brokkr.DDD.UoW;

/// <summary>
/// Represents a failed entity operation.
/// </summary>
/// <param name="Entity">Entity whose operation failed.</param>
/// <param name="ErrorCode">Code describing the kind of error.</param>
/// <param name="Message">Optional message describing the error.</param>
public sealed record EntityOperationFailure(object Entity, EntityOperationErrorCode ErrorCode, string? Message = null);
