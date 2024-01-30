using Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Features.Users;

/// <summary>
/// Validate reset password code
/// </summary>
public class ValidateResetPasswordCode
{
    /// <summary>
    /// Validate reset password code command
    /// </summary>
    public class Command : IRequest<Result>
    {
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        /// <summary>
        /// 
        /// </summary>
        public Validator()
        {
            RuleFor(d => d.Code).NotEmpty();
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
            var code = await db.ResetPasswordCodes.FirstOrDefaultAsync(d => d.Code == request.Code, cancellationToken);

            if (code == null)
                return new BadRequestError();

            if (code.ConsumedAt.HasValue)
                return new BadRequestError().AddFieldErrors(nameof(request.Code), "Código já utilizado");

            if (code.ExpiresAt < DateTime.UtcNow)
                return new BadRequestError().AddFieldErrors(nameof(request.Code), "Código expirado");

            return Result.Success;
        }
    }
}
