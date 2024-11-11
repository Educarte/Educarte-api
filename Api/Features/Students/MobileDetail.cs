using Api.Results.Diary;
using Api.Results.Menus;
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
/// Find student by id
/// </summary>
public class MobileDetail
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

        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// EndDate
        /// </summary>
        public DateTime EndDate { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty();
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
                .Include(x => x.LegalGuardian)
                    .ThenInclude(x => x.Address)
                .Include(x => x.Diaries.Where(x => (x.Time >= request.StartDate.Date && x.Time <= request.EndDate.Date) && !x.DeletedAt.HasValue))
                .Include(x => x.EmergencyContacts.Where(x => !x.DeletedAt.HasValue))
                .OnlyActives()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (student == null)
                return new NotFoundError("Estudante não encontrado.");

            var studentResult = student.Adapt<StudentResult>();

            var diaryClassrooms = db.Diaries
                .Where(x => (x.IsDiaryForAll || x.Classrooms.Any(y => y.Students.Any(z => z.Id == student.Id))) &&
                (x.Time.Date >= request.StartDate.Date && x.Time.Date <= request.EndDate.Date) &&
                !x.DeletedAt.HasValue)
                .ToList();

            if (diaryClassrooms.Any())
            {
                var diaries = diaryClassrooms.Adapt<List<DiarySimpleResult>>();
                studentResult.Diaries.AddRange(diaries);
            }

            var currentMenu = await db.Menus.FirstOrDefaultAsync(x => x.Status == Core.Enums.Status.Active && (DateTime.Now.Date >= x.StartDate.Date && DateTime.Now.Date <= x.ValidUntil.Date), cancellationToken);

            if (currentMenu != null)
                studentResult.CurrentMenu = currentMenu.Adapt<MenuResult>();

            return studentResult;
        }
    }
}
