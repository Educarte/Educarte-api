using Data;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Validators;

public class ClassroomExistenceValidator<T> : AsyncPropertyValidator<T, Guid>
{
    private const string _name = nameof(ClassroomExistenceValidator<T>);
    private readonly ApiDbContext db;

    public override string Name => _name;

    protected override string GetDefaultMessageTemplate(string errorCode)
                        => "A sala de aula não existe";

    public ClassroomExistenceValidator(ApiDbContext db)
    {
        this.db = db;
    }

    public override async Task<bool> IsValidAsync(ValidationContext<T> context, Guid value, CancellationToken cancellation)
    {
        return await db.Classrooms.AsNoTrackingWithIdentityResolution().AnyAsync(d => d.Id == value, cancellation);
    }
}
