using Core.Enums;
using Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Features.Menus;

/// <summary>
/// Active / Inactive menu
/// </summary>
public class ToggleActive
{
    /// <summary>
    /// Active / Inactive menu
    /// </summary>
    public class Command : IRequest<Result>
    {
        /// <summary>
        /// menu id 
        /// </summary>
        public Guid Id { get; set; }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var menu = await db.Menus.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (menu == null)
                return new NotFoundError("Menu não encontrado.");

            menu.Status = menu.Status == Status.Active ? Status.Inactive : Status.Active;

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}