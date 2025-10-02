using Brokkr.DDD.UoW;

namespace Brokkr.DDD.Saga;

public abstract partial class SagaBase
{
    /// <summary>
    /// Encapsulates repository and unit of work context for entity operations within a saga.
    /// </summary>
    /// <param name="Repository">The repository for entity operations.</param>
    /// <param name="UnitOfWork">The unit of work for transaction management.</param>
    protected sealed record EntityContext(IUnitOfWorkPatternRepository Repository, IUnitOfWork UnitOfWork)
    {
        /// <summary>
        /// Creates an EntityContext from a standalone repository that implements both interfaces.
        /// </summary>
        /// <param name="standaloneRepository">Repository that serves as both repository and unit of work.</param>
        public EntityContext(IStandaloneRepository standaloneRepository)
            : this(standaloneRepository, standaloneRepository)
        {
        }
    };

    /// <summary>
    /// Base record for saga operations that can be executed and compensated.
    /// </summary>
    /// <param name="Context">The entity context for the operation.</param>
    protected abstract record Operation(EntityContext Context)
    {
        /// <summary>
        /// Gets a value indicating whether this operation is asynchronous.
        /// </summary>
        public abstract bool IsAsync { get; }

        /// <summary>
        /// Executes the operation synchronously.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public abstract Task ExecuteAsync();

        /// <summary>
        /// Executes the compensation action synchronously.
        /// </summary>
        public abstract void Compensate();

        /// <summary>
        /// Executes the compensation action asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous compensation.</returns>
        public abstract Task CompensateAsync();
    }

    /// <summary>
    /// Represents a synchronous operation with optional compensation for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type this operation works with.</typeparam>
    /// <param name="Context">The entity context for the operation.</param>
    /// <param name="OperationAction">The synchronous operation to execute.</param>
    /// <param name="CompensationAction">Optional synchronous compensation action.</param>
    protected sealed record SyncOperation<TEntity>(
        EntityContext Context,
        Action<IUnitOfWorkPatternRepository<TEntity>> OperationAction,
        Action<IUnitOfWorkPatternRepository<TEntity>>? CompensationAction)
        : Operation(Context)
    {
        /// <inheritdoc />
        public override bool IsAsync => false;

        /// <inheritdoc />
        public override void Execute()
        {
            var repository = (IUnitOfWorkPatternRepository<TEntity>)Context.Repository;
            OperationAction(repository);
        }

        /// <inheritdoc />
        public override Task ExecuteAsync()
        {
            Execute();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override void Compensate()
        {
            if (CompensationAction != null)
            {
                var repository = (IUnitOfWorkPatternRepository<TEntity>)Context.Repository;
                CompensationAction(repository);
            }
        }

        /// <inheritdoc />
        public override Task CompensateAsync()
        {
            Compensate();
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Represents an asynchronous operation with optional compensation for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type this operation works with.</typeparam>
    /// <param name="Context">The entity context for the operation.</param>
    /// <param name="OperationAction">The asynchronous operation to execute.</param>
    /// <param name="CompensationAction">Optional asynchronous compensation action.</param>
    protected sealed record AsyncOperation<TEntity>(
        EntityContext Context,
        Func<IUnitOfWorkPatternRepository<TEntity>, Task> OperationAction,
        Func<IUnitOfWorkPatternRepository<TEntity>, Task>? CompensationAction)
        : Operation(Context)
    {
        /// <inheritdoc />
        public override bool IsAsync => true;

        /// <inheritdoc />
        public override void Execute()
        {
            ExecuteAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync()
        {
            var repository = (IUnitOfWorkPatternRepository<TEntity>)Context.Repository;
            await OperationAction(repository);
        }

        /// <inheritdoc />
        public override void Compensate()
        {
            CompensateAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public override async Task CompensateAsync()
        {
            if (CompensationAction != null)
            {
                var repository = (IUnitOfWorkPatternRepository<TEntity>)Context.Repository;
                await CompensationAction(repository);
            }
        }
    }
}
