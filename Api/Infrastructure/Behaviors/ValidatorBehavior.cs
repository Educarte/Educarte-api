using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Infrastructure.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TResponse : Nudes.Retornator.Core.IResult
                                                                                              where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    public class ValidatorErrorDetail
    {
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ValidatorErroDetailPropertyNameEqualityComparer : IEqualityComparer<ValidatorErrorDetail>
    {
        public bool Equals(ValidatorErrorDetail x, ValidatorErrorDetail y)
        {
            if (y is null || x is null) return false;

            return x.PropertyName.Equals(y.PropertyName);
        }

        public int GetHashCode(ValidatorErrorDetail obj) => obj.PropertyName.GetHashCode();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var failedValidations = new List<ValidationResult>();
        await Parallel.ForEachAsync(validators, cancellationToken, async (validator, token) =>
        {
            var result = await validator.ValidateAsync(request, token);
            if (!result.IsValid)
                failedValidations.Add(result);
        });

        if (failedValidations.Any())
        {
            var fieldErrors = new FieldErrors();
            var errors = failedValidations.SelectMany(f => f.Errors
                                                            .Select(e => new ValidatorErrorDetail()
                                                            {
                                                                PropertyName = e.PropertyName,
                                                                ErrorMessage = e.ErrorMessage,
                                                                PropertyValue = e.FormattedMessagePlaceholderValues?["PropertyValue"]?.ToString() ?? "",
                                                            })).GroupBy(d => d.PropertyName);

            foreach (var error in errors)
            {
                fieldErrors.Add(error.Key, error.Select(d => d.ErrorMessage).ToArray());
            }

            var result = Activator.CreateInstance<TResponse>();
            result.Error = new BadRequestError()
            {
                FieldErrors = fieldErrors
            };

            return result;
        }

        return await next();
    }
}
