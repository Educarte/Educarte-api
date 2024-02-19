using Api.Infrastructure;
using Api.Infrastructure.Services;
using Api.Infrastructure.Validators;
using Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Features.Users;

/// <summary>
/// Change User Password
/// </summary>
public class ResetPassword
{
    /// <summary>
    /// Reset User Password
    /// </summary>
    public class Command : IRequest<Result>
    {
        /// <summary>
        /// Id of user
        /// </summary>
        [BindNever]
        public Guid UserId { get; set; }

        /// <summary>
        /// New password
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirm password
        /// </summary>
        public string ConfirmPassword { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(ApiDbContext db)
        {
            RuleFor(d => d.ConfirmPassword).NotEmpty().Matches(x => x.NewPassword).WithMessage("Senha e Confirmação precisam ser iguais.");
            RuleFor(d => d.NewPassword).NotEmpty().SetValidator(new PasswordValidator<Command>());
        }
    }
    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApiDbContext db;
        private readonly IActor actor;
        private readonly HashService hashService;

        public Handler(ApiDbContext db, IActor actor, HashService hashService)
        {
            this.db = db;
            this.actor = actor;
            this.hashService = hashService;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await db.Users.FirstOrDefaultAsync(d => d.Id == request.UserId, cancellationToken);
            if (user == null)
                return new ForbiddenError();

            var (passwordHash, passwordSalt) = hashService.Encrypt(request.NewPassword);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
