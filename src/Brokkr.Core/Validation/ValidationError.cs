using System.Text.Json.Serialization;

using Brokkr.Core.Converters;

namespace Brokkr.Core.Validation;

/// <summary>
/// Base class for generic validation errors.
/// </summary>
public abstract record ValidationError
{
    /// <summary>
    /// Error code that identifies the type of validation error.
    /// </summary>
    [JsonPropertyName("errorCode")]
    [JsonConverter(typeof(PlainEnumStringConverter))]
    public Enum ErrorCode { get; init; }

    /// <summary>
    /// Human-readable error message describing the validation error.
    /// </summary>
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Identifier that specifies which field, property or etc caused the validation error.
    /// </summary>
    [JsonPropertyName("identifier")]
    public string? Identifier { get; init; }

    /// <summary>
    /// Collection of inner validation errors that are related to this error
    /// (e.g. when a property has multiple validation errors or multiple nested members with errors).
    /// </summary>
    [JsonPropertyName("innnerErrors")]
    public ValidationError[]? InnerErrors { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class.
    /// </summary>
    protected ValidationError(
        Enum errorCode,
        string? errorMessage = null,
        string? identifier = null,
        ValidationError[]? innerErrors = null)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Identifier = identifier;
        InnerErrors = innerErrors;
    }

    /// <inheritdoc/>
    public virtual bool Equals(ValidationError? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Equals(ErrorCode, other.ErrorCode)
            && ErrorMessage == other.ErrorMessage
            && Identifier == other.Identifier
            && (InnerErrors?.SequenceEqual(other.InnerErrors ?? []) ?? other.InnerErrors is null);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var arrayHash = InnerErrors?.Aggregate(0, (acc, item) => HashCode.Combine(acc, item.GetHashCode())) ?? 0;
        return HashCode.Combine(ErrorCode, ErrorMessage, Identifier, arrayHash);
    }
    
    /// <summary>
    /// Creates a new instance of <see cref="ValidationError{T}"/> with the specified parameters.
    /// </summary>
    /// <typeparam name="T">The enum type for the error code.</typeparam>
    /// <param name="errorCode"><see cref="ValidationError{T}.ErrorCode"/>.</param>
    /// <param name="errorMessage"><see cref="ErrorMessage"/></param>
    /// <param name="identifier"><see cref="Identifier"/></param>
    /// <param name="innerErrors"><see cref="InnerErrors"/></param>
    /// <returns>A new instance of <see cref="ValidationError{T}"/>.</returns>
    /// <remarks>
    /// This is a shorthand to create strongly-typed validation errors without explicitly specifying the enum type.
    /// </remarks>
    public static ValidationError<T> Create<T>(
        T errorCode,
        string? errorMessage = null,
        string? identifier = null,
        ValidationError[]? innerErrors = null)
        where T : Enum
    {
#pragma warning disable S1135 // suppressed until .NET 10 is available
        // TODO: make this an extension method in .NET 10
#pragma warning restore S1135
        return new ValidationError<T>(errorCode, errorMessage, identifier, innerErrors);
    }
}

/// <summary>
/// Generic validation error class with a strongly-typed error code.
/// </summary>
/// <typeparam name="T">The enum type that defines the possible error codes.</typeparam>
public record ValidationError<T> : ValidationError where T : Enum
{
    /// <summary>
    /// Gets the strongly-typed error code that identifies the type of validation error.
    /// </summary>
    [JsonPropertyName("errorCode")]
    public new T ErrorCode
    {
        get => (T)base.ErrorCode;
        init => base.ErrorCode = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError{T}"/> class.
    /// </summary>
    public ValidationError(
        T errorCode,
        string? errorMessage = null,
        string? identifier = null,
        ValidationError[]? innerErrors = null)
        : base(errorCode, errorMessage, identifier, innerErrors)
    {
    }
}
