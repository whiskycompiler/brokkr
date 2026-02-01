namespace Brokkr.DDD.ChangeTracking;

/// <summary>
/// Enumeration of states that a tracker entry can have.
/// </summary>
public enum TrackingState
{
    /// <summary>
    /// Tracking is disabled.
    /// </summary>
    Detached = 0,

    /// <summary>
    /// The instance exists and is tracked.
    /// Its current values are not changed from its original values.
    /// </summary>
    Unchanged = 1,

    /// <summary>
    /// The instance is new and is tracked.
    /// </summary>
    Added = 2,

    /// <summary>
    /// The instance exists and is tracked.
    /// Its current values are different from its original values.
    /// </summary>
    Modified = 3,

    /// <summary>
    /// The instance exists and is tracked.
    /// It has been marked for deletion.
    /// </summary>
    Deleted = 4,
}
