using Api.Infrastructure.Validators;
using Api.Results.Students;
using Core.Interfaces;
using Data;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;
using System.Text.Json.Serialization;

namespace Api.Features.Students;

/// <summary>
/// Edit an student
/// </summary>
public class Edit
{
    /// <summary>
    /// Edit and student command
    /// </summary>
    public class Command : IRequest<ResultOf<StudentResult>>
    {
        /// <summary>
        /// Student id
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid Id { get; set; }

        /// <summary>
        /// Name of student
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Legal guardian Id
        /// </summary>
        public Guid LegalGuardianId { get; set; }

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
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.LegalGuardianId).NotEmpty().SetAsyncValidator(legalGuardianExistenceValidator);
            RuleFor(x => x.ClassroomId).NotEmpty().SetAsyncValidator(classroomExistenceValidator);
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
            var student = await db.Students.OnlyActives().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (student == null)
                return new NotFoundError("Estudante não encontrado, tente novamente.");

            request.Adapt(student);

            await db.SaveChangesAsync(cancellationToken);

            return student.Adapt<StudentResult>();
        }
    }
}
