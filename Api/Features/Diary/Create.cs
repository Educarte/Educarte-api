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

    internal class Adapter : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Command, Core.Diary>();
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator(ApiDbContext db)
        {
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.FileUri).NotEmpty();
            RuleFor(x => x.IsDiaryForAll).NotEmpty();
            RuleFor(x => x.Time).NotEmpty();

            When(x => !x.IsDiaryForAll && x.StudentIds.Any(), () =>
            {
                RuleForEach(x => x.StudentIds).NotEmpty().SetAsyncValidator(new StudentExistenceValidator<Command>(db)).When(x => !x.ClassroomIds.Any()).WithMessage("Estudante não existe ou lista de salas vieram preenchidas.");
            });

            When(x => !x.IsDiaryForAll && x.ClassroomIds.Any(), () =>
            {
                RuleForEach(x => x.ClassroomIds).NotEmpty().SetAsyncValidator(new ClassroomExistenceValidator<Command>(db)).When(x => !x.StudentIds.Any()).WithMessage("Estudante não existe ou lista de salas vieram preenchidas.");
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

            diary.UserId = actor.UserId;

            if (!request.IsDiaryForAll && request.StudentIds.Any())
                diary.Students.AddRange(db.Students.Where(x => request.StudentIds.Contains(x.Id)));
            else if(!request.IsDiaryForAll && request.ClassroomIds.Any())
                diary.Classrooms.AddRange(db.Classrooms.Where(x => request.ClassroomIds.Contains(x.Id)));

            db.Diaries.Add(diary);
            await db.SaveChangesAsync(cancellationToken);

            return diary.Adapt<DiarySimpleResult>();
        }
    }
}
