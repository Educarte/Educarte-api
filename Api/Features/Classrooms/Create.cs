using Api.Infrastructure.Options;
using Api.Infrastructure.Services.Email;
using Api.Infrastructure.Services;
using Api.Infrastructure.Validators;
using Api.Results.Students;
using Core;
using Core.Enums;
using Core.Interfaces;
using Data;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;
using Api.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Options;
using Api.Results.Classroom;

namespace Api.Features.Classrooms;

/// <summary>
/// Create a classroom
/// </summary>
public class Create
{
    /// <summary>
    /// Create a classroom command
    /// </summary>
    public class Command : IRequest<ResultOf<ClassroomResult>>
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Max Student
        /// </summary>
        public int MaxStudents { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public Status Status { get; set; }
        
        /// <summary>
        /// Time
        /// </summary>
        public Time Time { get; set; }

        /// <summary>
        /// ClassroomType
        /// </summary>
        public ClassroomType ClassroomType { get; set; }

        /// <summary>
        /// TeacherIds
        /// </summary>
        public IList<Guid> TeacherIds { get; set; }

        /// <summary>
        /// StudentIds
        /// </summary>
        public IList<Guid> StudentIds { get; set; }
    }

    internal class Adapter : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Command, Classroom>();
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator(ApiDbContext db)
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.MaxStudents).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.Time).NotEmpty();
            RuleFor(x => x.ClassroomType).NotEmpty();
            RuleFor(x => x.Time).NotEmpty();

            RuleForEach(x => x.TeacherIds).NotEmpty().SetAsyncValidator(new TeacherExistenceValidator<Command>(db));
            RuleForEach(x => x.StudentIds).NotEmpty().SetAsyncValidator(new StudentExistenceValidator<Command>(db));
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<ClassroomResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<ClassroomResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var teachers = await db.Users
                .Where(x => request.TeacherIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var students = await db.Students
                .Where(x => request.StudentIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var classroom = request.Adapt<Classroom>();

            classroom.Students = students;
            classroom.Teachers = teachers;

            db.Classrooms.Add(classroom);
            await db.SaveChangesAsync(cancellationToken);

            return classroom.Adapt<ClassroomResult>();
        }
    }
}
