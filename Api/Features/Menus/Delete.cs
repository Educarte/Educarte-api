using Core.Interfaces;
using Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Features.Menus;

/// <summary>
/// Delete an menu
/// </summary>
public class Delete
{
    /// <summary>
    /// Delete and menu command
    /// </summary>
    public class Command : IRequest<Result>
    {
        /// <summary>
        /// menu Id
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
            var menu = await db.Menus.OnlyActives().FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (menu == null)
                return new NotFoundError("Cardápio não encontrado.");

            menu.DeletedAt = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}