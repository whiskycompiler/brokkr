using Brokkr.DDD.ChangeTracking;

namespace Brokkr.DDD.UnitTests;

public sealed class ChangeTrackerTests
{
    [Fact]
    public void AddOrUpdateEntry_AddsNewEntry_WithGivenState()
    {
        var tracker = new ChangeTracker();
        var ds = new DummyDataSet();
        var person = new Person
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Age = 28,
        };

        tracker.AddOrUpdateEntry(person, TrackingState.Added, ds);

        Assert.Single(tracker.Entries);
        var entry = tracker.Entries[0];
        Assert.Same(person, entry.TrackedInstance);
        Assert.Equal(TrackingState.Added, entry.State);
        Assert.Same(ds, entry.DataSet);
    }

    [Fact]
    public void AddOrUpdateEntry_UpdatesExistingEntryState()
    {
        var tracker = new ChangeTracker();
        var ds = new DummyDataSet();
        var person = new Person
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Age = 28,
        };

        tracker.AddOrUpdateEntry(person, TrackingState.Added, ds);
        tracker.AddOrUpdateEntry(person, TrackingState.Modified, ds);

        Assert.Single(tracker.Entries);
        Assert.Equal(TrackingState.Modified, tracker.Entries[0].State);
    }

    [Fact]
    public void TrackerEntry_DetectsModifications_FromSnapshot()
    {
        var ds = new DummyDataSet();
        var person = new Person
        {
            FirstName = "Alan",
            LastName = "Turing",
            Age = 35,
        };

        var entry = new TrackerEntry<Person>(person, TrackingState.Unchanged, ds);

        // Initially unchanged
        Assert.Equal(TrackingState.Unchanged, entry.State);

        // Modify instance -> should become Modified when State is observed
        person.Age++;
        Assert.Equal(TrackingState.Modified, entry.State);

        // Accept changes -> snapshot updated and state becomes Unchanged
        entry.AcceptChanges();
        Assert.Equal(TrackingState.Unchanged, entry.State);
    }

    [Fact]
    public void AcceptChanges_RemovesDeletedEntries_AndResetsStates()
    {
        var tracker = new ChangeTracker();
        var ds = new DummyDataSet();
        var a = new Person { FirstName = "A" };
        var b = new Person { FirstName = "B" };
        var c = new Person { FirstName = "C" };

        tracker.AddOrUpdateEntry(a, TrackingState.Unchanged, ds);
        tracker.AddOrUpdateEntry(b, TrackingState.Modified, ds);
        tracker.AddOrUpdateEntry(c, TrackingState.Deleted, ds);

        tracker.AcceptChanges();

        Assert.Equal(2, tracker.Entries.Count);
        var entryA = Assert.Single(tracker.Entries, e => ReferenceEquals(e.TrackedInstance, a));
        var entryB = Assert.Single(tracker.Entries, e => ReferenceEquals(e.TrackedInstance, b));
        Assert.Equal(TrackingState.Unchanged, entryA.State);
        Assert.Equal(TrackingState.Unchanged, entryB.State);
    }

    [Fact]
    public void AcceptChanges_ForSpecificInstances_OnlyAffectsProvided_Ones()
    {
        var tracker = new ChangeTracker();
        var ds = new DummyDataSet();
        var a = new Person { FirstName = "A" };
        var b = new Person { FirstName = "B" };

        tracker.AddOrUpdateEntry(a, TrackingState.Modified, ds);
        tracker.AddOrUpdateEntry(b, TrackingState.Modified, ds);

        tracker.AcceptChanges([a]);

        var entryA = Assert.Single(tracker.Entries, e => ReferenceEquals(e.TrackedInstance, a));
        var entryB = Assert.Single(tracker.Entries, e => ReferenceEquals(e.TrackedInstance, b));
        Assert.Equal(TrackingState.Unchanged, entryA.State);
        Assert.Equal(TrackingState.Modified, entryB.State);
    }
}
