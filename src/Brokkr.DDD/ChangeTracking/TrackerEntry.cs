using Brokkr.DDD.UoW;

namespace Brokkr.DDD.ChangeTracking;

/// <summary>
/// Represents an entry in the tracker containing the tracked entity and metadata.
/// </summary>
public sealed class TrackerEntry<TTrackedInstance> : ITrackerEntry
    where TTrackedInstance : class, ITrackable<TTrackedInstance>
{
    private TrackingState _state;
    private TTrackedInstance? _snapshot;

    /// <inheritdoc/>
    public object TrackedInstance => TypedInstance;

    /// <summary>
    /// The tracked instance.
    /// </summary>
    public TTrackedInstance TypedInstance { get; }

    /// <inheritdoc/>
    public TrackingState State
    {
        get
        {
            DetectChanges();
            return _state;
        }
    }

    /// <inheritdoc/>
    public IDataSet DataSet { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackerEntry{T}"/> class.
    /// </summary>
    public TrackerEntry(TTrackedInstance instance, TrackingState state, IDataSet dataSet)
    {
        _state = state;
        TypedInstance = instance;
        DataSet = dataSet;

        if (state is TrackingState.Unchanged)
        {
            _snapshot = TypedInstance.DeepClone();
        }
    }

    /// <inheritdoc/>
    public object? GetSnapshot()
    {
        return _snapshot?.DeepClone();
    }

    /// <inheritdoc/>
    public void AcceptChanges()
    {
        switch (State)
        {
            case TrackingState.Detached:
            case TrackingState.Unchanged:
                break;
            case TrackingState.Modified:
            case TrackingState.Added:
                TransitionEntryState(TrackingState.Unchanged);
                break;
            case TrackingState.Deleted:
                TransitionEntryState(TrackingState.Detached);
                break;
            default:
                throw new InvalidOperationException($"Entry has unknown state '{State}'!");
        }
    }

    /// <inheritdoc/>
    public void TransitionEntryState(TrackingState newState)
    {
        if (
            // detaching removes the snapshot because tracking is not necessary and remote DB state unknown
            newState == TrackingState.Detached

            // if modified state is forced remove the snapshot to prevent it from reverting
            || (_state is TrackingState.Unchanged && newState is TrackingState.Modified))
        {
            _snapshot = null;
        }

        // this case forces acceptance of changes or add as the current DB state
        else if (_state is TrackingState.Added or TrackingState.Modified && newState is TrackingState.Unchanged)
        {
            _snapshot = TypedInstance.DeepClone();
        }

        _state = newState;
    }

    private void DetectChanges()
    {
        if (_snapshot == null)
        {
            return;
        }

        switch (_state)
        {
            case TrackingState.Added:
            case TrackingState.Deleted:
            case TrackingState.Detached:
                return;

            case TrackingState.Unchanged:
            case TrackingState.Modified:
            {
                _state = TypedInstance.Equals(_snapshot) ? TrackingState.Unchanged : TrackingState.Modified;
                return;
            }

            default:
                throw new InvalidOperationException($"Unknown entry state '{_state}'!");
        }
    }
}
