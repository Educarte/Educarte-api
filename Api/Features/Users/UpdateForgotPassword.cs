using Api.Infrastructure.Services;
using Api.Infrastructure.Validators;
using Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Features.Users;

/// <summary>
/// Update forgot password
/// </summary>
public class UpdateForgotPassword
{
    /// <summary>
    /// Update forgot password command
    /// </summary>
    public class Command : IRequest<Result>
    {
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

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
        /// <summary>
        /// 
        /// </summary>
        public Validator()
        {
            RuleFor(d => d.ConfirmPassword).NotEmpty().Matches(x => x.NewPassword).WithMessage("Senha e Confirmação precisam ser iguais.");
            RuleFor(d => d.NewPassword).NotEmpty().SetValidator(new PasswordValidator<Command>());
            RuleFor(d => d.Code).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApiDbContext db;
        private readonly HashService hashService;

        public Handler(ApiDbContext db, HashService hashService)
        {
            this.db = db;
            this.hashService = hashService;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var code = await db.ResetPasswordCodes.Include(d => d.User)
                                                  .FirstOrDefaultAsync(d => d.Code == request.Code, cancellationToken);

            if (code == null)
                return new BadRequestError();

            if (code.ConsumedAt.HasValue)
                return new BadRequestError().AddFieldErrors(nameof(request.Code), "Código já utilizado");

            if (code.ExpiresAt < DateTime.UtcNow)
                return new BadRequestError().AddFieldErrors(nameof(request.Code), "Código expirado");

            code.ConsumedAt = DateTime.UtcNow;

            var user = code.User;
            var (passwordHash, passwordSalt) = hashService.Encrypt(request.NewPassword);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }

}
