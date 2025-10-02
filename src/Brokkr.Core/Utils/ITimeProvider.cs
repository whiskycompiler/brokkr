namespace Brokkr.Core.Utils;

/// <summary>
/// Describes a component to provide the current time.
/// </summary>
public interface ITimeProvider
{
    /// <inheritdoc cref="DateTimeOffset.UtcNow"/>
    DateTimeOffset UtcNow { get; }
}