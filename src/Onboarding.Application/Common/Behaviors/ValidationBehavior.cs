// Application/Common/Behaviors/ValidationBehavior.cs
using FluentValidation;
using MediatR;
using Onboarding.Application.Common.Models;

namespace Onboarding.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        // Validate request
        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            // Handle validation failure
            var resultType = typeof(TResponse);

            if (resultType.IsGenericType &&
                resultType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                // Create failure result
                var failureMethod = typeof(Result<>)
                    .MakeGenericType(resultType.GenericTypeArguments[0])
                    .GetMethod("Failure");

                if (failureMethod != null)
                {
                    var errorMessages = failures.Select(f => f.ErrorMessage).ToList();
                    return (TResponse)failureMethod.Invoke(null,
                        new object[] { "Validation failed", errorMessages })!;
                }
            }

            throw new ValidationException(failures);
        }

        return await next();
    }
}