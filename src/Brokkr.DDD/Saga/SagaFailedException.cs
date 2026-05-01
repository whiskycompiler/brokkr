namespace Brokkr.DDD.Saga;

/// <summary>
/// Exception that occurs when a saga fails to complete successfully.
/// </summary>
public class SagaFailedException : Exception
{
    /// <summary>
    /// Collection of exceptions that occurred trying to rollback the saga using compensating actions.
    /// </summary>
    public IReadOnlyCollection<Exception> RollbackExceptions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SagaFailedException"/> class.
    /// </summary>
    /// <param name="message">The message of the exception.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="rollbackExceptions">
    ///     Collection of exceptions that occurred trying to rollback the saga using compensating actions.
    /// </param>
    public SagaFailedException(
        string message,
        Exception innerException,
        IReadOnlyCollection<Exception>? rollbackExceptions = null)
        : base(message, innerException)
    {
        RollbackExceptions = rollbackExceptions ?? [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SagaFailedException"/> class.
    /// </summary>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="rollbackExceptions">
    ///     Collection of exceptions that occurred trying to rollback the saga using compensating actions.
    /// </param>
    public SagaFailedException(
        Exception innerException,
        IReadOnlyCollection<Exception> rollbackExceptions)
        : base(
            rollbackExceptions.Count > 0
                ? "Saga failed to complete successfully and failed during rollback. See InnerException and RollbackExceptions for details."
                : "Saga failed to complete successfully. See InnerException for details.",
            innerException)
    {
        RollbackExceptions = rollbackExceptions;
    }
}
