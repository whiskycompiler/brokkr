namespace Brokkr.Core.Utils;

/// <summary>
/// Default implementation for <see cref="ITimeProvider"/>.
/// </summary>
public class TimeProvider : ITimeProvider
{
    /// <inheritdoc/>
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
