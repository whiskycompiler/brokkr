using System.Runtime.CompilerServices;

using Xunit.Sdk;

namespace Brokkr.Testing.XUnit.Assertions;

/// <summary>
/// Assertion extensions for bool values.
/// </summary>
public static class BoolExtensions
{
    /// <summary>
    /// Provides extension methods for performing assertions on boolean values.
    /// </summary>
    /// <param name="value">The value to check.</param>
    extension(bool value)
    {
        /// <summary>
        /// Asserts that the value is true.
        /// </summary>
        /// <param name="valueExpression">Ignore - filled by CallerArgumentExpression.</param>
        public void AssertTrue([CallerArgumentExpression(nameof(value))] string valueExpression = "")
        {
            if (!value)
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to be true".
                     Actual: '{value}'
                     """);
            }
        }

        /// <summary>
        /// Asserts that the value is false.
        /// </summary>
        /// <param name="valueExpression">Ignore - filled by CallerArgumentExpression.</param>
        public void AssertFalse([CallerArgumentExpression(nameof(value))] string valueExpression = "")
        {
            if (value)
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to be false".
                     Actual: '{value}'
                     """);
            }
        }
    }
}
