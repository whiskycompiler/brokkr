using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;

using Xunit.Sdk;

namespace Brokkr.Testing.XUnit.Assertions;

/// <summary>
/// Assertion extensions for objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Asserts that the value is not null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="valueExpression">Ignore - filled by CallerArgumentExpression.</param>
    public static void AssertNotNull<T>(
        [NotNull] this T? value,
        [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
#pragma warning disable S2955 // this is an excplicit null check
        if (value == null)
#pragma warning restore S2955
        {
            throw new XunitException(
                $"""
                 Expected '{valueExpression}' to be not null".
                 Actual: '{value}'
                 """);
        }
    }
    
    /// <summary>
    /// Extension methods for nullable objects.
    /// </summary>
    /// <param name="value">The nullable value to check.</param>
    /// <typeparam name="T">The type of the nullable value.</typeparam>
    extension<T>(T? value)
    {
        /// <summary>
        /// Asserts that the value is null.
        /// </summary>
        /// <param name="valueExpression">Ignore - filled by CallerArgumentExpression.</param>
        public void AssertNull([CallerArgumentExpression(nameof(value))] string valueExpression = "")
        {
#pragma warning disable S2955 // this is an excplicit null check
            if (value != null)
#pragma warning restore S2955
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to be null".
                     Actual: '{value}'
                     """);
            }
        }

        /// <summary>
        /// Asserts that the value is equal to the specified value.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="valueExpression">Ignore - filled by CallerArgumentExpression.</param>
        /// <param name="expectedExpression">Ignore - filled by CallerArgumentExpression.</param>       
        public void AssertEqual(
            T? expected,
            [CallerArgumentExpression(nameof(value))] string valueExpression = "",
            [CallerArgumentExpression(nameof(expected))] string expectedExpression = "")
        {
            if (!Equals(value, expected))
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to be equal to '{expectedExpression}'.
                     Expected: {JsonSerializer.Serialize(expected)}
                     Actual: {JsonSerializer.Serialize(value)}
                     """);
            }
        }

        /// <summary>
        /// Asserts that the value is not equal to the specified value.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="valueExpression">Ignore - filled by CallerArgumentExpression.</param>
        /// <param name="expectedExpression">Ignore - filled by CallerArgumentExpression.</param>
        public void AssertNotEqual(
            T? expected,
            [CallerArgumentExpression(nameof(value))] string valueExpression = "",
            [CallerArgumentExpression(nameof(expected))] string expectedExpression = "")
        {
            if (Equals(value, expected))
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to be not equal to '{expectedExpression}'.
                     Expected: {JsonSerializer.Serialize(expected)}
                     Actual: {JsonSerializer.Serialize(value)}
                     """);
            }
        }
    }
}
