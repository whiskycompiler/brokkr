namespace Brokkr.DDD.UoW;

/// <summary>
/// Represents an entity with a unique identifier.
/// </summary>
/// <typeparam name="TId">The type of the entity's unique identifier.</typeparam>
public interface IEntity<out TId>
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    TId Id { get; }
}
