using System.Diagnostics.CodeAnalysis;

using Brokkr.DDD.UoW;

namespace Brokkr.DDD.Saga;

/// <summary>
/// Base implementation of a saga that manages operations and their compensations in a transaction-safe manner.
/// </summary>
public abstract partial class SagaBase : ISaga
{
    /// <summary>
    /// Maps entity types to their corresponding contexts required for operations.
    /// </summary>
    protected Dictionary<Type, EntityContext> EntityContextMap { get; } = [];

    /// <summary>
    /// List of operations to be executed in order.
    /// </summary>
    protected List<Operation> Operations { get; } = [];

    /// <summary>
    /// Safely retrieves the entity context for a given entity type.
    /// </summary>
    /// <param name="entityType">The entity type to get context for.</param>
    /// <returns>The entity context for the specified type.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the entity type is not known to this saga.</exception>
    protected EntityContext SafeGetEntityContext(Type entityType)
    {
        if (EntityContextMap.TryGetValue(entityType, out var context))
        {
            return context;
        }

        throw new SagaException(
            SagaErrorCode.UnknownEntityType,
            $"Entity type {entityType.FullName} is not known by this saga.");
    }

    /// <inheritdoc />
    public void AddOperation<TEntity>(
        Action<IUnitOfWorkPatternRepository<TEntity>> operation,
        Action<IUnitOfWorkPatternRepository<TEntity>>? compensationAction = null)
    {
        var context = SafeGetEntityContext(typeof(TEntity));
        Operations.Add(new SyncOperation<TEntity>(context, operation, compensationAction));
    }

    /// <inheritdoc />
    public void AddOperation<TEntity>(
        Func<IUnitOfWorkPatternRepository<TEntity>, Task> operation,
        Func<IUnitOfWorkPatternRepository<TEntity>, Task>? compensationAction = null)
    {
        var context = SafeGetEntityContext(typeof(TEntity));
        Operations.Add(new AsyncOperation<TEntity>(context, operation, compensationAction));
    }

    /// <summary>
    /// Executes all operations in order and handles compensation rollback if any operation fails.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    // ReSharper disable MethodHasAsyncOverload
    [SuppressMessage("Major Code Smell",
        "S6966:Awaitable method should be used",
        Justification = "Method checks explicit indicators to decide if async or sync must be used!")]
    public async Task SaveTrackedChanges(CancellationToken cancellationToken = new())
    {
        Exception? exception = null;
        var failedIndex = -1;
        for (var i = 0; i < Operations.Count; i++)
        {
            var operation = Operations[i];

            try
            {
                if (operation.IsAsync)
                {
                    await operation.ExecuteAsync();
                }
                else
                {
                    operation.Execute();
                }

                await operation.Context.UnitOfWork.SaveTrackedChanges(cancellationToken);
            }
            catch (Exception e)
            {
                exception = e;
                failedIndex = i;
                break;
            }
        }

        if (exception != null)
        {
            var rollbackExceptions = new List<Exception>();
            for (var i = failedIndex - 1; i >= 0; i--)
            {
                try
                {
                    var operation = Operations[i];
                    if (operation.IsAsync)
                    {
                        await operation.CompensateAsync();
                    }
                    else
                    {
                        operation.Compensate();
                    }

                    // no cancellation token forwarding because rollbacks should not be cancelled
                    await operation.Context.UnitOfWork.SaveTrackedChanges(CancellationToken.None);
                }
                catch (Exception e)
                {
                    rollbackExceptions.Add(e);
                    // we do not break and stop the rollbacks here, because we want to rollback as many operations
                    // as possible, even if one or multiple rollbacks fail
                }
            }

            throw new SagaFailedException(exception, rollbackExceptions);
        }

        // ReSharper restore MethodHasAsyncOverload
    }
}
