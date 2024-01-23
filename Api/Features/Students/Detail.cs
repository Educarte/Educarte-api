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

    internal class Validator : AbstractValidator<Query>
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
            var students = await db.Students
                .Include(x => x.Classroom)
                    .ThenInclude(x => x.Diaries)
                .Include(x => x.ContractedHours)
                .Include(x => x.LegalGuardians)
                .Include(x => x.Diaries)
                .Include(x => x.EmergencyContacts)
                .Include(x => x.AccessControls)
                .OnlyActives()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
            if (students == null)
                return new NotFoundError("Usuário não encontrado.");

            return students.Adapt<StudentResult>();
        }
    }
}
