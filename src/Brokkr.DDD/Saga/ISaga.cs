using Brokkr.DDD.UoW;

namespace Brokkr.DDD.Saga;

/// <summary>
/// Represents a saga that coordinates multiple operations in independent systems
/// with compensation logic for distributed transactions.
/// </summary>
public interface ISaga : IUnitOfWork
{
    /// <summary>
    /// Adds a synchronous operation with optional compensation to the saga.
    /// </summary>
    /// <typeparam name="TEntity">The entity type the operation works with.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="compensationAction">Optional compensation action to execute if rollback is needed.</param>
    void AddOperation<TEntity>(
        Action<IUnitOfWorkPatternRepository<TEntity>> operation,
        Action<IUnitOfWorkPatternRepository<TEntity>>? compensationAction = null);

    /// <summary>
    /// Adds an asynchronous operation with optional compensation to the saga.
    /// </summary>
    /// <typeparam name="TEntity">The entity type the operation works with.</typeparam>
    /// <param name="operation">The asynchronous operation to execute.</param>
    /// <param name="compensationAction">Optional asynchronous compensation action to execute if rollback is needed.</param>
    void AddOperation<TEntity>(
        Func<IUnitOfWorkPatternRepository<TEntity>, Task> operation,
        Func<IUnitOfWorkPatternRepository<TEntity>, Task>? compensationAction = null);
}
