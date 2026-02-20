using FluentValidation;
using MediatR;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using System.Reflection;

namespace SmartGrid.Application.Common.Behaviors;

internal class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken ct)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, ct)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                var errorsDictionary = failures
                    .GroupBy(f => f.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(f => f.ErrorMessage).ToArray()
                    );

                var serializedErrors = System.Text.Json.JsonSerializer.Serialize(errorsDictionary);

                return CreateValidationResult(serializedErrors);
            }
        }

        return await next(ct);
    }

    private TResponse CreateValidationResult(string message)
    {
        var responseType = typeof(TResponse);

        if (responseType == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(message, ErrorType.Validation);
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = responseType.GetMethod("Failure",
                BindingFlags.Public | BindingFlags.Static);

            if (failureMethod != null)
            {
                var result = failureMethod.Invoke(null, [message, ErrorType.Validation]);
                return (TResponse)result!;
            }
        }

        throw new ValidationException(message);
    }
}