using Brokkr.DDD.UoW;

using Microsoft.EntityFrameworkCore;

namespace Brokkr.DDD.EFCore;

/// <summary>
/// Base repository class to adapt EF Core repositories to the Brokkr.DDD interfaces.
/// </summary>
/// <typeparam name="TContext">The type of the Entity Framework <see cref="DbContext"/> used by this repository.</typeparam>
/// <typeparam name="TEntity">The type of the entity that the repository operates on.</typeparam>
public abstract class BaseEfRepository<TContext, TEntity> : IStandaloneRepository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    /// <summary>
    /// EF Core <see cref="DbContext"/> used by this repository to interact with the database.
    /// </summary>
    protected TContext Context { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEfRepository{T1,T2}"/> class.
    /// </summary>
    protected BaseEfRepository(TContext context)
    {
        Context = context;
    }

    /// <inheritdoc/>
    public void AddEntity(TEntity entity)
    {
        Context.Set<TEntity>().Add(entity);
    }

    /// <inheritdoc/>
    public void UpdateEntity(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }

    /// <inheritdoc/>
    public void RemoveEntity(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }

    /// <inheritdoc/>
    public virtual Task SaveTrackedChanges(CancellationToken cancellationToken = default)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }
}
