using Data;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Validators;

public class TeacherExistenceValidator<T> : AsyncPropertyValidator<T, Guid>
{
    private const string _name = nameof(TeacherExistenceValidator<T>);
    private readonly ApiDbContext db;

    public override string Name => _name;

    protected override string GetDefaultMessageTemplate(string errorCode)
                        => "O usuário não existe ou não é um professor ou um colaborador.";

    public TeacherExistenceValidator(ApiDbContext db)
    {
        this.db = db;
    }

    public override async Task<bool> IsValidAsync(ValidationContext<T> context, Guid value, CancellationToken cancellation)
    {
        return await db.Users.AsNoTrackingWithIdentityResolution().AnyAsync(d => d.Id == value && (d.Profile == Core.Enums.Profile.Teacher || d.Profile == Core.Enums.Profile.Employee), cancellation);
    }
}
