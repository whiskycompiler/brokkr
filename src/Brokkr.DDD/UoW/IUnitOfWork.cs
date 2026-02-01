namespace Brokkr.DDD.UoW;

/// <summary>
/// Describes a unit of work used to save tracked changes of all connected data.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all tracked changes.
    /// </summary>
    Task SaveTrackedChanges(CancellationToken cancellationToken = default);
}
