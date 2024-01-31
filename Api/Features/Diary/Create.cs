using Api.Infrastructure.Validators;
using Core;
using Data;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.Core;
using Api.Results.Diary;
using Api.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace Api.Features.Diary;

/// <summary>
/// Create
/// </summary>
public class Create
{
    /// <summary>
    /// Create command
    /// </summary>
    public class Command : IRequest<ResultOf<DiarySimpleResult>>
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// FileUri
        /// </summary>
        public string FileUri { get; set; }

        /// <summary>
        /// IsDiaryForAll
        /// </summary>
        public bool IsDiaryForAll { get; set; }

        /// <summary>
        /// Time
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// StudentIds
        /// </summary>
        public IList<Guid> StudentIds { get; set; }

        /// <summary>
        /// ClassroomIds
        /// </summary>
        public IList<Guid> ClassroomIds { get; set; }
    }

    public class Adapter : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Command, Core.Diary>();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(ApiDbContext db)
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.IsDiaryForAll).NotNull();
            RuleFor(x => x.Time).NotEmpty();

            When(x => !x.IsDiaryForAll && !x.StudentIds.IsNullOrEmpty(), () =>
            {
                RuleForEach(x => x.StudentIds).NotEmpty().SetAsyncValidator(new StudentExistenceValidator<Command>(db)).When(x => x.ClassroomIds.IsNullOrEmpty()).WithMessage("Estudante não existe ou lista de salas vieram preenchidas.");
            });

            When(x => !x.IsDiaryForAll && !x.ClassroomIds.IsNullOrEmpty(), () =>
            {
                RuleForEach(x => x.ClassroomIds).NotEmpty().SetAsyncValidator(new ClassroomExistenceValidator<Command>(db)).When(x => x.StudentIds.IsNullOrEmpty()).WithMessage("Turma não existe ou lista de estudantes vieram preenchidas.");
            });
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<DiarySimpleResult>>
    {
        private readonly ApiDbContext db;
        private readonly IActor actor;

        public Handler(ApiDbContext db, IActor actor)
        {
            this.db = db;
            this.actor = actor;
        }

        public async Task<ResultOf<DiarySimpleResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var diary = request.Adapt<Core.Diary>();

            if (!request.IsDiaryForAll && !request.StudentIds.IsNullOrEmpty())
            {
                var studentsToAdd = request.StudentIds.Select(studentId => new Student { Id = studentId }).ToList();
                db.Students.AttachRange(studentsToAdd);

                diary.Students = new();
                diary.Students.AddRange(studentsToAdd);
            }
            else if (!request.IsDiaryForAll && !request.ClassroomIds.IsNullOrEmpty())
            {
                var classroomsToAdd = request.ClassroomIds.Select(classroomId => new Classroom { Id = classroomId }).ToList();
                db.Classrooms.AttachRange(classroomsToAdd);

                diary.Classrooms = new();
                diary.Classrooms.AddRange(classroomsToAdd);
            }

            db.Diaries.Add(diary);
            await db.SaveChangesAsync(cancellationToken);

            return diary.Adapt<DiarySimpleResult>();
        }
    }
}
