namespace Brokkr.Core.Validation;

/// <summary>
/// Exception caused when validation fails.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Collection of validation errors that caused the exception.
    /// </summary>
    public IReadOnlyCollection<ValidationError> ValidationErrors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    public ValidationException(
        IReadOnlyCollection<ValidationError> validationErrors,
        string? message = null)
        : base(message)
    {
        ValidationErrors = validationErrors;
    }
}
