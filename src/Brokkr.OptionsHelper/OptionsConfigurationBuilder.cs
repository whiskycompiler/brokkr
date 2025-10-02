using System.Reflection;

using FluentValidation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Brokkr.OptionsHelper;

// ReSharper disable UnusedMember.Global
/// <summary>
/// Helper to configure app config options with automatic fluent validation.
/// Uses the validation logic from the net core option pattern and automatically
/// registers fluent validators for the given option types.
/// </summary>
public class OptionsConfigurationBuilder
{
    private readonly IConfiguration _configuration;
    private readonly IServiceCollection _serviceCollection;

    private readonly HashSet<Type> _typesToConfigureWithValidation = new();
    private readonly HashSet<Type> _typesToConfigureWithoutValidation = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsConfigurationBuilder"/> class.
    /// </summary>
    public OptionsConfigurationBuilder(
        IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        _serviceCollection = serviceCollection;
        _configuration = configuration;
    }

    /// <summary>
    /// Adds the specified type as a configured option.
    /// </summary>
    /// <typeparam name="TOptions">Option to configure with validation.</typeparam>
    /// <returns>Current instance for chaining.</returns>
    public OptionsConfigurationBuilder AddOption<TOptions>()
        where TOptions : class
    {
        var type = typeof(TOptions);
        if (_typesToConfigureWithValidation.Contains(type) || !_typesToConfigureWithoutValidation.Add(type))
        {
            Console.WriteLine($"Tried to register option '{type.Name}' again!");
        }

        return this;
    }

    /// <summary>
    /// Adds the specified type as a configured option with fluent validation.
    /// </summary>
    /// <typeparam name="TOptions">Option to configure with validation.</typeparam>
    /// <returns>Current instance for chaining.</returns>
    public OptionsConfigurationBuilder AddOptionWithValidation<TOptions>()
        where TOptions : class
    {
        var type = typeof(TOptions);
        if (_typesToConfigureWithoutValidation.Contains(type) || !_typesToConfigureWithValidation.Add(type))
        {
            Console.WriteLine($"Tried to register option '{type.Name}' again! Validation might not be added!");
        }

        return this;
    }

    /// <summary>
    /// Configures all added options and registers the net core validator using fluent validators.
    /// </summary>
    /// <returns>Current instance for chaining.</returns>
    public OptionsConfigurationBuilder Configure()
    {
        var setupExceptions = new List<Exception>();
        using var serviceProvider = _serviceCollection.BuildServiceProvider();

        foreach (var optionType in _typesToConfigureWithValidation)
        {
            ConfigureOptionWithValidation(optionType, serviceProvider, setupExceptions);
        }

        foreach (var optionType in _typesToConfigureWithoutValidation)
        {
            ConfigureOption(optionType);
        }

        if (setupExceptions.Count != 0)
        {
            throw new AggregateException(setupExceptions);
        }

        return this;
    }

    /// <summary>
    /// Validates all options configured with a validator now and throws all errors as an aggregate
    /// if any should occur.
    /// </summary>
    /// <exception cref="AggregateException">Validation exceptions from all options with configured validation.</exception>
    /// <returns>Current instance for chaining.</returns>
    public OptionsConfigurationBuilder ValidateNow()
    {
        var exceptions = new List<Exception>();

        // get all option types with a configured validator
        var options = _serviceCollection
            .Where(serviceDescriptor =>
            {
                if (serviceDescriptor.ImplementationInstance?.GetType().IsGenericType != true)
                {
                    return false;
                }

                return serviceDescriptor.ImplementationInstance.GetType()
                    .GetGenericTypeDefinition()
                    .GetInterfaces()
                    .Any(a =>
                        a.IsGenericType && a.GetGenericTypeDefinition() == typeof(IValidateOptions<>));
            })
            .Select(serviceDescriptor =>
                serviceDescriptor.ImplementationInstance!.GetType().GetGenericArguments()[0]);

        // instantiate all found option types and access their value property to trigger the validation
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        foreach (var optionType in options)
        {
            try
            {
                var option = serviceProvider.GetRequiredService(typeof(IOptions<>).MakeGenericType(optionType));
                var valueProperty = option.GetType().GetProperty(nameof(IOptions<object>.Value));

                if (valueProperty == null || valueProperty.GetMethod == null)
                {
                    exceptions.Add(new InvalidOperationException(
                        $"Could not find the IOption<>.Value property getter on resolved option type {option.GetType().FullName}!"));
                    continue;
                }

                valueProperty.GetMethod.Invoke(option, null);
            }
            catch (TargetInvocationException tex)
            {
                if (tex.InnerException is not OptionsValidationException ovex)
                {
                    throw;
                }

                exceptions.Add(ovex);
            }
        }

        if (exceptions.Count != 0)
        {
            throw new AggregateException(exceptions);
        }

        return this;
    }

    /// <summary>
    /// Configues an option. Basically this call via generic: <code>services.Configure&lt;Config&gt;(Configuration.GetSection("Config"))</code>
    /// </summary>
    private void ConfigureOption(Type optionType)
    {
        var optionTypeName = optionType.Name;

        // get method to configure option
        var configMethod = GetOptionConfigurationMethod();
        if (configMethod == null)
        {
            throw new NotSupportedException(
                "Could not find the method to configure the option!\n" +
                "Please make sure your option types are all classes and you are using .NET 6!");
        }

        configMethod.MakeGenericMethod(optionType).Invoke(
            null,
            new object[]
            {
                _serviceCollection,
                _configuration.GetSection(optionTypeName),
                new Action<BinderOptions>(options => { options.BindNonPublicProperties = true; }),
            });
    }

    /// <summary>
    /// Configures an option like <see cref="ConfigureOption"/> but with added option validators.
    /// </summary>
    private void ConfigureOptionWithValidation(Type optionType,
        IServiceProvider serviceProvider,
        List<Exception> setupExceptions)
    {
        ConfigureOption(optionType);

        // get fluent validator for option type - save thrown exceptions to throw them as aggregate
        IValidator fluentValidator;
        try
        {
            fluentValidator = GetRegisteredFluentValidator(optionType, serviceProvider);
        }
        catch (InvalidOperationException ex)
        {
            setupExceptions.Add(ex);
            return;
        }

        // build net core adapter that uses the fluent validator
        var validatorConstructorInfo = typeof(FluentOptionValidator<>)
            .MakeGenericType(optionType)
            .GetConstructor([fluentValidator.GetType()]);
        if (validatorConstructorInfo == null)
        {
            throw new NotImplementedException(
                "Could not find necessary generic constructor for FluentOptionValidator<>!");
        }

        object[] parameters = [fluentValidator];
        var validator = validatorConstructorInfo.Invoke(parameters);

        // register validator
        _serviceCollection.AddSingleton(typeof(IValidateOptions<>).MakeGenericType(optionType), validator);
    }


    /// <summary>
    /// Determines the fluent validator for a given type and gets an instance from the given service provider.
    /// </summary>
    /// <param name="optionType">Type for validation.</param>
    /// <param name="serviceProvider">Service provider to resolve the validator for the given type.</param>
    /// <returns>Fluent validator for the given type.</returns>
    /// <exception cref="InvalidOperationException">When no fitting validator can be found.</exception>
    private static IValidator GetRegisteredFluentValidator(Type optionType, IServiceProvider serviceProvider)
    {
        // try to find the validator type for the given option type
        var validators = AssemblyScanner
            .FindValidatorsInAssemblyContaining(optionType)
            .Where(w => w.InterfaceType.GenericTypeArguments.Contains(optionType))
            .ToArray();
        if (validators.Length != 1)
        {
            throw new InvalidOperationException(
                $"Incorrect amount of validators found in assembly!\nType: '{optionType.FullName}'\nExpected: 1\nFound: {validators.Length}");
        }

        // try to resolve the validator
        var validatorType = validators[0].ValidatorType;
        if (serviceProvider.GetService(validatorType) is not IValidator validator)
        {
            throw new InvalidOperationException(
                $"The necessary validator '{validatorType.FullName}' is not registered in the ServiceCollection!");
        }

        return validator;
    }

    /// <summary>
    /// Gets the .NET Core IServiceCollection extension method to configure options.
    /// </summary>
    /// <returns>Method info of the method.</returns>
    private static MethodInfo? GetOptionConfigurationMethod()
    {
        return typeof(OptionsConfigurationServiceCollectionExtensions)
            .GetMethod(
                nameof(OptionsConfigurationServiceCollectionExtensions.Configure),
                BindingFlags.Static | BindingFlags.Public,
                null,
                CallingConventions.Any,
                [
                    typeof(IServiceCollection),
                    typeof(IConfiguration),
                    typeof(Action<BinderOptions>),
                ],
                null);
    }
}
