namespace Brokkr.DDD.ChangeTracking;

// Suppress: Seal class 'TrackableObject' or implement 'IEqualityComparer<T>' instead.
// Reason: class is abstract - IEqualityComparer<T> is implemented by child classes
#pragma warning disable S4035

/// <summary>
/// Default implementation of a trackable object.
/// </summary>
/// <typeparam name="T">The type of the trackable object itself.</typeparam>
public abstract class TrackableObject<T> : ITrackable<T> where T : class
{
    /// <inheritdoc/>
    public abstract T DeepClone();

    /// <inheritdoc/>
    public abstract bool Equals(T? other);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        return Equals((T)obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        // Suppress: GetHashCode should not throw an exception
        // Reason: This is the default design for ITrackable objects.
        // They are mutable and you should never rely on their hash code.
#pragma warning disable S3877
        throw new NotSupportedException(
            "Trackable objects are intended to be mutable and do not provide a hash code! If you still want to support this override the base implementation of GetHashCode() in your class.");
#pragma warning restore S3877
    }
}
