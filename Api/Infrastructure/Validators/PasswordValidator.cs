using FluentValidation;
using FluentValidation.Validators;

namespace Api.Infrastructure.Validators;

public class PasswordValidator<T> : PropertyValidator<T, string>
{
    private const string _name = nameof(PasswordValidator<T>);

    public override string Name => _name;

    public PasswordValidator()
    {
    }
    public override bool IsValid(ValidationContext<T> context, string value)
    {
        return !string.IsNullOrEmpty(value) &&
               value.Length >= 8 &&
               value.Any(char.IsUpper) &&
               value.Any(char.IsLower) &&
               value.Any(char.IsDigit);
    }
    protected override string GetDefaultMessageTemplate(string errorCode)
                        => "A Senha deve ter pelo menos 8 caracteres e conter pelo menos uma letra maiúscula, uma letra minúscula e um dígito numérico.";
}
