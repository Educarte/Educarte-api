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

namespace Api.Features.Menus;

/// <summary>
/// Find menu by id
/// </summary>
public class Detail
{
    /// <summary>
    /// Find menu by id
    /// </summary>
    public class Query : IRequest<ResultOf<MenuResult>>
    {
        /// <summary>
        /// menu id 
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

    internal class Handler : IRequestHandler<Query, ResultOf<MenuResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<MenuResult>> Handle(Query request, CancellationToken cancellationToken)
        {
            var menu = await db.Menus
                .OnlyActives()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (menu == null)
                return new NotFoundError("Cardápio não encontrado.");

            return menu.Adapt<MenuResult>();
        }
    }
}
