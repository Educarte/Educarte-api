using Api.Infrastructure.Validators;
using Api.Results.Students;
using Core;
using Core.Enums;
using Data;
using FluentValidation;
using Mapster;
using MediatR;
using Nudes.Retornator.Core;

namespace Api.Features.Students;

/// <summary>
/// Create a student
/// </summary>
public class Create
{
    /// <summary>
    /// Create a student command
    /// </summary>
    public class Command : IRequest<ResultOf<StudentResult>>
    {
        /// <summary>
        /// Name of student
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Legal guardian Id
        /// </summary>
        public IList<Guid> LegalGuardianIds { get; set; }

        /// <summary>
        /// Classroom Id
        /// </summary>
        public Guid ClassroomId { get; set; }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator(LegalGuardianExistenceValidator<Command> legalGuardianExistenceValidator,
            ClassroomExistenceValidator<Command> classroomExistenceValidator)
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleForEach(x => x.LegalGuardianIds).NotEmpty().SetAsyncValidator(legalGuardianExistenceValidator);
            RuleFor(x => x.ClassroomId).NotEmpty().SetAsyncValidator(classroomExistenceValidator);
        }
    }

    internal class Adapter : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Command, Student>();
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<StudentResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<StudentResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var student = request.Adapt<Student>();
            db.Students.Add(student);
            await db.SaveChangesAsync(cancellationToken);

            return student.Adapt<StudentResult>();
        }
    }
}
