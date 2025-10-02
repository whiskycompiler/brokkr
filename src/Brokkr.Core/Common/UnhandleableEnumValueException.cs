namespace Brokkr.Core.Common;

/// <summary>
/// Exception to throw when an unhandleable enum value is encountered.
/// </summary>
public sealed class UnhandleableEnumValueException : Exception
{
    /// <summary>
    /// Name of the invalid argument if there is one.
    /// </summary>
    public string? ArgumentName { get; }

    /// <summary>
    /// Enum value that could not be handled.
    /// </summary>
    public Enum? EnumValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandleableEnumValueException"/> class.
    /// </summary>
    public UnhandleableEnumValueException(Enum? enumValue, string? argumentName = null)
        : base(BuildMessage(enumValue, argumentName))
    {
        EnumValue = enumValue;
        ArgumentName = argumentName;
    }

    private static string BuildMessage(Enum? enumValue, string? argumentName)
    {
        return (enumValue, argumentName) switch
        {
            (null, null) =>
                "Encountered unhandlable enum value 'null'.",

            (not null, null) =>
                $"Encountered unhandlable enum value '{enumValue}' of type '{enumValue.GetType().FullName}'.",

            (null, not null)
                => $"Argument '{argumentName}' contains unhandlable enum value 'null'.",

            (_, _) =>
                $"Argument '{argumentName}' contains an unhandlable enum value '{enumValue}' of type '{enumValue.GetType().FullName}'.",
        };
    }
}
