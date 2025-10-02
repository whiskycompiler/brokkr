namespace Brokkr.Core.Common;

/// <summary>
/// Base exception using an error code based on an enum.
/// </summary>
public abstract class EnumErrorCodeException : Exception
{
    /// <summary>
    /// Enum error code of the exception.
    /// </summary>
    public Enum ErrorCode { get; set; }

    /// <summary>
    /// Indicates whether the exception is a critical issue (e.g. not automatically recoverable).
    /// </summary>
    public bool IsCritical { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumErrorCodeException"/> class.
    /// </summary>
    protected EnumErrorCodeException(
        Enum errorCode,
        string? message = null,
        bool isCritical = false)
        : base(message ?? $"Error Code: {errorCode.ToString()}")
    {
        ErrorCode = errorCode;
        IsCritical = isCritical;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumErrorCodeException"/> class.
    /// </summary>
    protected EnumErrorCodeException(
        Enum errorCode,
        Exception innerException,
        string? message = null,
        bool isCritical = false)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        IsCritical = isCritical;
    }
}

/// <summary>
/// Typed base exception using an error code based on an enum.
/// </summary>
public abstract class EnumErrorCodeException<TEnum> : EnumErrorCodeException where TEnum : Enum
{
    /// <summary>
    /// Enum error code of the exception.
    /// </summary>
    public new TEnum ErrorCode
    {
        get => (TEnum)base.ErrorCode;
        set => base.ErrorCode = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumErrorCodeException{TEnum}"/> class.
    /// </summary>
    protected EnumErrorCodeException(
        TEnum errorCode,
        string? message = null,
        bool isCritical = false)
        : base(errorCode, message, isCritical)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumErrorCodeException{TEnum}"/> class.
    /// </summary>
    protected EnumErrorCodeException(
        TEnum errorCode,
        Exception innerException,
        string? message = null,
        bool isCritical = false)
        : base(errorCode, innerException, message, isCritical)
    {
    }
}
