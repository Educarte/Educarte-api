using Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Features.Users;

/// <summary>
/// Active / Inactive users
/// </summary>
public class ToggleActive
{
    /// <summary>
    /// Active / Inactive user
    /// </summary>
    public class Command : IRequest<Result>
    {
        /// <summary>
        /// user id 
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
            var user = await db.Users.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (user == null)
                return new NotFoundError("Usuário não encontrado.");

            user.Status = user.Status == Core.Enums.Status.Active ? Core.Enums.Status.Inactive :
                                                                    Core.Enums.Status.Active;

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}