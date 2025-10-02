using Brokkr.DDD.UoW;

namespace Brokkr.DDD.ChangeTracking;

/// <summary>
/// General implementation of a change tracker.
/// </summary>
public sealed class ChangeTracker
{
    private readonly List<ITrackerEntry> _entries = [];

    /// <summary>
    /// Collection of all tracked entries.
    /// </summary>
    public IReadOnlyList<ITrackerEntry> Entries => _entries;

    /// <summary>
    /// Adds a new entry or updates an existing entry in the change tracker with the specified tracking state.
    /// </summary>
    public void AddOrUpdateEntry<TInstance>(
        TInstance instance,
        TrackingState newState,
        IDataSet dataSet)
        where TInstance : class, ITrackable<TInstance>
    {
        var entry = _entries.SingleOrDefault(c => ReferenceEquals(c.TrackedInstance, instance));
        if (entry != null)
        {
            entry.TransitionEntryState(newState);
        }
        else
        {
            _entries.Add(new TrackerEntry<TInstance>(instance, newState, dataSet));
        }
    }

    /// <summary>
    /// Accepts the current state of all tracked instances and sets them to <see cref="TrackingState.Unchanged"/>.
    /// </summary>
    public void AcceptChanges()
    {
        foreach (var trackedEntity in _entries.ToArray())
        {
            AcceptChangesToTrackedEntity(trackedEntity);
        }
    }

    /// <summary>
    /// Accepts the current state of all given instances and sets them to <see cref="TrackingState.Unchanged"/>.
    /// Untracked instances are ignored.
    /// </summary>
    public void AcceptChanges(IEnumerable<object> instances)
    {
        var entries = _entries.ToArray();
        foreach (var instance in instances)
        {
            var entry = Array.Find(entries, entry => ReferenceEquals(entry.TrackedInstance, instance));
            if (entry != null)
            {
                AcceptChangesToTrackedEntity(entry);
            }
        }
    }

    private void AcceptChangesToTrackedEntity(ITrackerEntry trackerEntry)
    {
        if (trackerEntry.State is TrackingState.Deleted)
        {
            _entries.Remove(trackerEntry);
        }

        trackerEntry.AcceptChanges();
    }
}
