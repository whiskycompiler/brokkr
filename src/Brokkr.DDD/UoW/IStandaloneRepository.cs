namespace Brokkr.DDD.UoW;

/// <summary>
/// Describes a repository which can be used as its own unit of work.
/// </summary>
public interface IStandaloneRepository : IUnitOfWork, IUnitOfWorkPatternRepository;

/// <summary>
/// Describes a repository which can be used as its own unit of work.
/// </summary>
public interface IStandaloneRepository<in TEntity> : IStandaloneRepository, IUnitOfWorkPatternRepository<TEntity>
{
}
