using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Marketplace.Api.Validation;

/// <summary>
/// Validates action arguments against any registered FluentValidation validators at the boundary
/// (Constitution II). Failures raise a ValidationAppException → 422 with per-field details.
/// </summary>
public sealed class FluentValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new List<ValidationError>();

        foreach (object argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (serviceProvider.GetService(validatorType) is IValidator validator)
            {
                var result = await validator.ValidateAsync(new ValidationContext<object>(argument));
                if (!result.IsValid)
                {
                    errors.AddRange(
                        result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)));
                }
            }
        }

        if (errors.Count > 0)
        {
            throw new ValidationAppException(errors);
        }

        await next();
    }
}
