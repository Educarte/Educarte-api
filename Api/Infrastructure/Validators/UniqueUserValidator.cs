using Data;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Validators;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class UniqueUserValidator<T> : AsyncPropertyValidator<T, string>
{
    private readonly ApiDbContext apiDbContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiDbContext"></param>
    public UniqueUserValidator(ApiDbContext apiDbContext)
    {
        this.apiDbContext = apiDbContext;
    }

    /// <summary>
    /// 
    /// </summary>
    public override string Name => nameof(UniqueUserValidator<T>);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "A user with e-mail {PropertyValue} already exists";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async override Task<bool> IsValidAsync(ValidationContext<T> context, string value, CancellationToken cancellationToken)
    {
        return !(await apiDbContext.Users.AnyAsync(d => d.Email == value, cancellationToken));
    }
}
