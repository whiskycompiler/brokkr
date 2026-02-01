using System.Runtime.CompilerServices;

using Xunit.Sdk;

namespace Brokkr.Testing.XUnit.Assertions;

/// <summary>
/// Assertion extensions for tasks.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Asserts that the task throws an exception of the specified type and returns it if it does.
    /// </summary>
    /// <param name="task">The task to await and check for the expected exception.</param>
    /// <param name="taskExpression">Ignore - filled by CallerArgumentExpression.</param>
    /// <typeparam name="T">The expected exception type.</typeparam>
    /// <returns>The thrown exception of type T.</returns>
    /// <exception cref="XunitException">Thrown if the task does not throw the expected exception.</exception>
    public static async Task<T> AssertThrows<T>(
        this Task task,
        [CallerArgumentExpression("task")] string taskExpression = "")
        where T : Exception
    {
        Exception? exception = null;
        try
        {
            await task;
        }
        catch (Exception e)
        {
            exception = e;
        }

        if (exception is T typedException)
        {
            return typedException;
        }

        var expectedType = typeof(T);
        throw new XunitException(
            $"""
             Expected '{taskExpression}' to throw an exception of type '{expectedType.Name}'.
             Expected: {expectedType.FullName}
             Actual: {exception?.GetType().FullName ?? "no exception thrown"}
             """);

    }
}
