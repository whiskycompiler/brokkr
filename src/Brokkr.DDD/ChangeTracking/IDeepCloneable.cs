namespace Brokkr.DDD.ChangeTracking;

/// <summary>
/// Describes the capability to create an identical copy of an instance and all contained data.
/// </summary>
/// <remarks>
/// The created copy should not contain any mutable references of any of the original data.
/// </remarks>
public interface IDeepCloneable<out T>
{
    /// <summary>
    /// Creates an identical copy of the original and all its data.
    /// </summary>
    T DeepClone();
}