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
/// Find student by id with details of access controls
/// </summary>
public class DetailAccessControl
{
    /// <summary>
    /// Find student by id with details of access controls query
    /// </summary>
    public class Query : IRequest<ResultOf<StudentSimpleResult>>
    {
        /// <summary>
        /// Student id 
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid Id { get; set; }
    }

    internal class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Query, ResultOf<StudentSimpleResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<StudentSimpleResult>> Handle(Query request, CancellationToken cancellationToken)
        {
            var student = await db.Students
                .Include(x => x.Classroom)
                .Include(x => x.ContractedHours)
                .Include(x => x.LegalGuardians)
                .Include(x => x.AccessControls)
                .OnlyActives()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (student == null)
                return new NotFoundError("Estudante não encontrado.");

            var accessControlsGroup = student.AccessControls.GroupBy(x => x.Time);

            var studentResult = student.Adapt<StudentSimpleResult>();

            return studentResult;
        }
    }
}
