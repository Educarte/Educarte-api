using Data;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Validators;

public class UserExistenceValidator<T> : AsyncPropertyValidator<T, Guid>
{
    private const string _name = nameof(UserExistenceValidator<T>);
    private readonly ApiDbContext db;

    public override string Name => _name;

    public UserExistenceValidator(ApiDbContext db)
    {
        this.db = db;
    }

    public override Task<bool> IsValidAsync(ValidationContext<T> context, Guid value, CancellationToken cancellation)
    {
        return db.Users.AsNoTrackingWithIdentityResolution().AnyAsync(d => d.Id == value, cancellation);
    }
}
