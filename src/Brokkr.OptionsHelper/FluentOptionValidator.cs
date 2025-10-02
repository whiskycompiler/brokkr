using FluentValidation;

using Microsoft.Extensions.Options;

namespace Brokkr.OptionsHelper;

/// <summary>
/// Generic configuration option validator using fluent validators
/// </summary>
/// <typeparam name="TOptions">Options type to validate.</typeparam>
public class FluentOptionValidator<TOptions> : IValidateOptions<TOptions>
    where TOptions : class
{
    private readonly AbstractValidator<TOptions> _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentOptionValidator{TOptions}"/> class.
    /// </summary>
    /// <param name="validator">Fluent validator used to validate the options.</param>
    public FluentOptionValidator(AbstractValidator<TOptions> validator)
    {
        _validator = validator;
    }

    /// <inheritdoc/>
    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        var validationResult = _validator.Validate(options);
        if (validationResult.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        var errors = validationResult.Errors.Select(s => s.ErrorMessage).ToList();
        return ValidateOptionsResult.Fail(errors);
    }
}
