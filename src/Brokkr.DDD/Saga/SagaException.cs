using Brokkr.Core.Common;

namespace Brokkr.DDD.Saga;

/// <summary>
/// Error codes for <see cref="SagaException"/>.
/// </summary>
public enum SagaErrorCode
{
    /// <summary>
    /// Indicates that the operation failed because the entity type is not known by the saga.
    /// </summary>
    UnknownEntityType,
}

/// <summary>
/// Represents errors that occur during the execution of saga-related operations.
/// </summary>
public class SagaException : EnumErrorCodeException<SagaErrorCode>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SagaException"/> class.
    /// </summary>
    public SagaException(
        SagaErrorCode errorCode,
        string? message = null,
        bool isCritical = false)
        : base(errorCode, message, isCritical)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SagaException"/> class.
    /// </summary>
    public SagaException(
        SagaErrorCode errorCode,
        Exception innerException,
        string? message = null,
        bool isCritical = false)
        : base(errorCode, innerException, message, isCritical)
    {
    }
}
