using Api.Infrastructure.Options;
using Api.Infrastructure.Services.Email;
using Api.Infrastructure.Services;
using Api.Infrastructure.Validators;
using Api.Results.Students;
using Core;
using Core.Enums;
using Core.Interfaces;
using Data;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;
using Api.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Options;
using Api.Results.Classroom;
using Api.Results.Menus;

namespace Api.Features.Menus;

/// <summary>
/// Create a classroom
/// </summary>
public class Create
{
    /// <summary>
    /// Create a classroom command
    /// </summary>
    public class Command : IRequest<ResultOf<MenuResult>>
    {
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

    public class Adapter : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Command, Menu>();
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
            var menu = request.Adapt<Menu>();

            db.Menus.Add(menu);
            await db.SaveChangesAsync(cancellationToken);

            return menu.Adapt<MenuResult>();
        }
    }
}
