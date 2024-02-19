using Api.Results.Diary;
using Api.Results.Menus;
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

namespace Api.Features.Diary;

/// <summary>
/// Find by id
/// </summary>
public class Detail
{
    /// <summary>
    /// Find by id
    /// </summary>
    public class Query : IRequest<ResultOf<DiaryResult>>
    {
        /// <summary>
        /// Id 
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

    internal class Handler : IRequestHandler<Query, ResultOf<DiaryResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<DiaryResult>> Handle(Query request, CancellationToken cancellationToken)
        {
            var diary = await db.Diaries
                .Include(x => x.Students)
                .Include(x => x.Classrooms)
                    .ThenInclude(x => x.Teachers)
                .OnlyActives()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (diary == null)
                return new NotFoundError("Nota não encontrada.");

            return diary.Adapt<DiaryResult>();
        }
    }
}
