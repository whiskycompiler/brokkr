namespace Brokkr.Core.Validation;

/// <summary>
/// Extension methods for <see cref="ValidationError"/>.
/// </summary>
public static class ValidationErrorExtensions
{
    extension(ValidationError)
    {
        /// <summary>
        /// Creates a new instance of <see cref="ValidationError{T}"/> with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The enum type for the error code.</typeparam>
        /// <param name="errorCode"><see cref="ValidationError{T}.ErrorCode"/>.</param>
        /// <param name="errorMessage"><see cref="ValidationError.ErrorMessage"/></param>
        /// <param name="identifier"><see cref="ValidationError.Identifier"/></param>
        /// <param name="innerErrors"><see cref="ValidationError.InnerErrors"/></param>
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
            return new ValidationError<T>(errorCode, errorMessage, identifier, innerErrors);
        }
    }
}
