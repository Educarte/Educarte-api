using FluentValidation;

namespace Api.Infrastructure.Extensions
{
    public static class FluentValidatorExtensions
    {
        public static IRuleBuilderOptions<T, DateTime> MustBeValidDateTime<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
        {
            return ruleBuilder.Must(date => date > DateTime.MinValue).WithMessage("A data e hora inválidas.");
        }

    }
}
