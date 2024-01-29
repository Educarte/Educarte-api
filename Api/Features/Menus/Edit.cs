using Api.Infrastructure.Validators;
using Api.Results.Classroom;
using Api.Results.Menus;
using Core;
using Core.Enums;
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
/// Edit an classroom
/// </summary>
public class Edit
{
    /// <summary>
    /// Edit and classroom command
    /// </summary>
    public class Command : IRequest<ResultOf<MenuResult>>
    {
        /// <summary>
        /// Classroom id
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File Uri
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// ValidUntil
        /// </summary>
        public DateTime ValidUntil { get; set; }
    }

    internal class Adapter : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Command, Classroom>();
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator(ApiDbContext db)
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Uri).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.ValidUntil).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<MenuResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<MenuResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var menu = await db.Menus
                .OnlyActives()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (menu == null)
                return new NotFoundError("Cardápio não encontrado.");

            request.Adapt(menu);

            await db.SaveChangesAsync(cancellationToken);

            return menu.Adapt<MenuResult>();
        }
    }
}
