using Api.Results.AccessControl;
using Api.Results.Classroom;
using Api.Results.ContractedHours;
using Api.Results.Diary;
using Api.Results.Students;
using Api.Results.Users;
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
/// Find student by id with details of access controls
/// </summary>
public class DetailAccessControl
{
    /// <summary>
    /// Find student by id with details of access controls query
    /// </summary>
    public class Query : IRequest<ResultOf<AccessControlResult>>
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
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// EndDate
        /// </summary>
        public DateTime? EndDate { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Query, ResultOf<AccessControlResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<AccessControlResult>> Handle(Query request, CancellationToken cancellationToken)
        {
            var student = await db.Students
                .Include(x => x.Classroom)
                .Include(x => x.ContractedHours)
                .Include(x => x.LegalGuardian)
                .Include(x => x.AccessControls.Where(x => !x.DeletedAt.HasValue))
                .OnlyActives()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (student == null)
                return new NotFoundError("Estudante não encontrado.");

            if (request.StartDate.HasValue)
                student.AccessControls = student.AccessControls.Where(x => x.Time.Date >= request.StartDate.Value).ToList();
            
            if (request.EndDate.HasValue)
                student.AccessControls = student.AccessControls.Where(x => x.Time.Date <= request.EndDate.Value).ToList();

            var accessControlGroup = new AccessControlResult
            {
                AccessControlsByDate = student.AccessControls.GroupBy(x => x.Time.Date).Select(x => new Results.AccessControl.AccessControl
                {
                    Date = x.Key,
                    AccessControls = x.Select(y => new AccessControlSimpleResult
                    {
                        Id = y.Id,
                        AccessControlType = y.AccessControlType,
                        Time = y.Time
                    }).ToList(),
                    ContractedHour = student.ContractedHours.FirstOrDefault(y => y.EndDate.HasValue ? x.Key <= y.EndDate.Value.Date : true)?.Adapt<ContractedHourResult>()
                }).ToList(),
                LegalGuardian = student.LegalGuardian.Adapt<UserSimpleResult>(),
                Student = student.Adapt<StudentBasicResult>(),
                Classroom = student.Classroom.Adapt<ClassroomBasicResult>()
            };

            foreach (var accessControl in accessControlGroup.AccessControlsByDate)
            {
                accessControl.DailySummary = (accessControl.AccessControls.FirstOrDefault(x => x.AccessControlType == Core.Enums.AccessControlType.Exit)?.Time - accessControl.AccessControls.FirstOrDefault(x => x.AccessControlType == Core.Enums.AccessControlType.Entrance)?.Time) - TimeSpan.FromHours((double)accessControl.ContractedHour.Hours);
                accessControlGroup.Summary += accessControl.DailySummary.HasValue ? accessControl.DailySummary.Value : TimeSpan.Zero;
            }

            return accessControlGroup;
        }
    }
}
