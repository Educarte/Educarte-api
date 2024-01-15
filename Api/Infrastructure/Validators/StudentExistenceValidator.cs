using Data;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Validators;

public class StudentExistenceValidator<T> : AsyncPropertyValidator<T, Guid>
{
    private const string _name = nameof(StudentExistenceValidator<T>);
    private readonly ApiDbContext db;

    public override string Name => _name;

    public StudentExistenceValidator(ApiDbContext db)
    {
        this.db = db;
    }

    public override Task<bool> IsValidAsync(ValidationContext<T> context, Guid value, CancellationToken cancellation)
    {
        return db.Students.AsNoTrackingWithIdentityResolution().AnyAsync(d => d.Id == value, cancellation);
    }
}
