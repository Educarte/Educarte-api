using Api.Results.Classroom;
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

namespace Api.Features.Classrooms;

/// <summary>
/// Find classroom by id
/// </summary>
public class Detail
{
    /// <summary>
    /// Find classroom by id
    /// </summary>
    public class Query : IRequest<ResultOf<ClassroomResult>>
    {
        /// <summary>
        /// Classroom id 
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

    internal class Handler : IRequestHandler<Query, ResultOf<ClassroomResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<ClassroomResult>> Handle(Query request, CancellationToken cancellationToken)
        {
            var classroom = await db.Classrooms
                .Include(x => x.Teachers)
                .Include(x => x.Diaries)
                .Include(x => x.Students)
                .OnlyActives()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (classroom == null)
                return new NotFoundError("Turma não encontrado.");

            return classroom.Adapt<ClassroomResult>();
        }
    }
}
