using Api.Results.Diary;
using Api.Results.Students;
using Core;
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
/// Find student by id
/// </summary>
public class Detail
{
    /// <summary>
    /// Find student by id
    /// </summary>
    public class Query : IRequest<ResultOf<StudentResult>>
    {
        /// <summary>
        /// Student id 
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid Id { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Query, ResultOf<StudentResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<StudentResult>> Handle(Query request, CancellationToken cancellationToken)
        {
            var student = await db.Students
                .Include(x => x.Classroom)
                    .ThenInclude(x => x.Diaries)
                .Include(x => x.Classroom)
                    .ThenInclude(x => x.Teachers)
                .Include(x => x.ContractedHours)
                .Include(x => x.LegalGuardians.Where(x => !x.DeletedAt.HasValue))
                .Include(x => x.Diaries.Where(x => !x.DeletedAt.HasValue))
                .Include(x => x.EmergencyContacts)
                .Include(x => x.AccessControls.Where(x => !x.DeletedAt.HasValue))
                .OnlyActives()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (student == null)
                return new NotFoundError("Estudante não encontrado.");

            var studentResult = student.Adapt<StudentResult>();

            var diaryClassrooms = db.Diaries
                .Where(x => x.IsDiaryForAll || x.Classrooms.Any(y => y.Students.Any(z => z.Id == student.Id)))
                .ToList();

            if (diaryClassrooms.Any())
            {
                var diaries = diaryClassrooms.Adapt<List<DiarySimpleResult>>();
                studentResult.Diaries.AddRange(diaries);
            }

            return studentResult;
        }
    }
}
