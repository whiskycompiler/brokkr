using System.Runtime.CompilerServices;

using Xunit.Sdk;

namespace Brokkr.Testing.XUnit.Assertions;

/// <summary>
/// Assertion extensions for bool values.
/// </summary>
public static class BoolExtensions
{
    extension(bool value)
    {
        /// <summary>
        /// Asserts that the value is true.
        /// </summary>
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
