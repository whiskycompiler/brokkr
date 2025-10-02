namespace Brokkr.DDD.ChangeTracking;

/// <summary>
/// Describes the capabilities of an object required for change tracking.
/// </summary>
public interface ITrackable<T> : IDeepCloneable<T>, IEquatable<T>;
