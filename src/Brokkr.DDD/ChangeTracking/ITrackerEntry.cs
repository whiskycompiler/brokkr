using Brokkr.DDD.UoW;

namespace Brokkr.DDD.ChangeTracking;

/// <summary>
/// Describes an entry in a change tracker.
/// </summary>
public interface ITrackerEntry
{
    /// <summary>
    /// Instance that is being tracked.
    /// </summary>
    object TrackedInstance { get; }

    /// <summary>
    /// State of the tracked instance.
    /// </summary>
    TrackingState State { get; }

    /// <summary>
    /// Dataset to which the tracked entity belongs.
    /// </summary>
    IDataSet DataSet { get; }

    /// <summary>
    /// Gets the snapshot representing the original state of the instance.
    /// </summary>
    object? GetSnapshot();

    /// <summary>
    /// Accepts all changes to the instance as the new <see cref="TrackingState.Unchanged"/> state.
    /// </summary>
    void AcceptChanges();

    /// <summary>
    /// Transitions the tracker entry into the specified state.
    /// </summary>
    void TransitionEntryState(TrackingState newState);
}
