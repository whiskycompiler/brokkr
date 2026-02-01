using System.Runtime.CompilerServices;
using System.Text.Json;

using Xunit.Sdk;

namespace Brokkr.Testing.XUnit.Assertions;

/// <summary>
/// Assertion extensions for enumerables.
/// </summary>
public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> value)
    {
        /// <summary>
        /// Asserts that the enumerable or string is empty.
        /// </summary>
        public void AssertEmpty([CallerArgumentExpression(nameof(value))] string valueExpression = "")
        {
            if (value is null)
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to be empty.
                     Actual Value: null
                     """);
            }

            if (value is string { Length: not 0 } str)
            {
                // for strings we print out the actual content
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to be empty.
                     Actual Value: {str}
                     """);
            }

            var length = value.Count();
            if (length != 0)
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to be empty.
                     Actual Length: {length}
                     """);
            }
        }

        /// <summary>
        /// Asserts that the enumerable or string is not empty.
        /// </summary>
        public void AssertNotEmpty([CallerArgumentExpression(nameof(value))] string valueExpression = "")
        {
            if (value is null)
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to be not empty.
                     Actual Value: null
                     """);
            }

            if (value is string { Length: 0 })
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to be not empty.
                     Actual Value: ""
                     """);
            }

            var length = value.Count();
            if (length == 0)
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to be not empty.
                     Actual Length: 0
                     """);
            }
        }

        /// <summary>
        /// Asserts that the enumerable has the exact specified length.
        /// </summary>
        public void AssertExactLength(
            int expectedLength,
            [CallerArgumentExpression(nameof(value))] string valueExpression = "")
        {
            if (value is null)
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to contain exactly {expectedLength} items."
                     Actual Value: null
                     """);
            }

            if (value is string str && str.Length != expectedLength)
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to be exactly {expectedLength} characters long."
                     Actual Length: {str.Length}
                     Actual Value: {str}
                     """);
            }

            var length = value.Count();
            if (length != expectedLength)
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to contain exactly {expectedLength} items."
                     Actual Length: {length}
                     """);
            }
        }

        /// <summary>
        /// Asserts that the enumerable contains the specified item.
        /// </summary>
        public void AssertContains(
            T? item,
            [CallerArgumentExpression(nameof(value))] string valueExpression = "")
        {
            if (value is null)
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to contain '{item}'.
                     Actual Value: null
                     """);
            }

            if (value is string str && !str.Contains(str))
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to contain '{str}'.
                     Actual Value: {value}
                     """);
            }

            var materialized = value.ToArray();
            if (!materialized.Contains(item))
            {
                throw new XunitException(
                    $"""
                     Expected '{valueExpression}' to contain '{item}'.
                     Actual Value: {JsonSerializer.Serialize(materialized)}
                     """);
            }
        }
    }
}
