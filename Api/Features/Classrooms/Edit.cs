using Api.Infrastructure.Validators;
using Api.Results.Classroom;
using Core;
using Core.Enums;
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

namespace Api.Features.Classrooms;

/// <summary>
/// Edit an classroom
/// </summary>
public class Edit
{
    /// <summary>
    /// Edit and classroom command
    /// </summary>
    public class Command : IRequest<ResultOf<ClassroomResult>>
    {
        /// <summary>
        /// Classroom id
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid Id { get; set; }

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

    public class Validator : AbstractValidator<Command>
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
            var classroom = await db.Classrooms
                .Include(x => x.Teachers)
                .Include(x => x.Students)
                .OnlyActives()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (classroom == null)
                return new NotFoundError("Estudante não encontrado, tente novamente.");

            request.Adapt(classroom);

            var teachersToAdd = request.TeacherIds.Except(classroom.Teachers.Select(x => x.Id))
                              .Select(teacherId => new User { Id = teacherId }).ToList();
            db.Users.AttachRange(teachersToAdd);

            classroom.Teachers.AddRange(teachersToAdd);
            classroom.Teachers.RemoveAll(classroom.Teachers.Where(m => !request.TeacherIds.Contains(m.Id)).Contains);

            var studentsToAdd = request.StudentIds.Except(classroom.Students.Select(x => x.Id))
                              .Select(studentId => new Student { Id = studentId }).ToList();
            db.Students.AttachRange(studentsToAdd);

            classroom.Students.AddRange(studentsToAdd);
            classroom.Students.RemoveAll(classroom.Students.Where(m => !request.TeacherIds.Contains(m.Id)).Contains);

            await db.SaveChangesAsync(cancellationToken);

            return classroom.Adapt<ClassroomResult>();
        }
    }
}
