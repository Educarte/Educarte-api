using Api.Infrastructure.Options;
using Api.Infrastructure.Services.Email;
using Api.Infrastructure.Services.Interfaces;
using Core;
using Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Features.Users;

/// <summary>
/// Mobile Request Reset Password
/// </summary>
public class MobileRequestResetPassword
{
    /// <summary>
    /// Mobile Request Reset Password command
    /// </summary>
    public class Command : IRequest<Result>
    {
        /// <summary>
        /// Email of user
        /// </summary>
        public string Email { get; set; }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(d => d.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }

    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApiDbContext db;
        private readonly IEmailService emailService;
        private readonly ResetPasswordOptions resetPasswordOptions;

        public Handler(ApiDbContext db, IEmailService emailService, IOptionsSnapshot<ResetPasswordOptions> resetPasswordOptions)
        {
            this.db = db;
            this.emailService = emailService;
            this.resetPasswordOptions = resetPasswordOptions.Value;
        }
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await db.Users.FirstOrDefaultAsync(d => d.Email == request.Email, cancellationToken);

            if (user == null)
                return new BadRequestError("Usuário não encontrado.");


            var resetPasswordCode = new ResetPasswordCode
            {
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddMinutes(resetPasswordOptions.CodeExpirationTime),
                Code = new Random().Next(1000, 10000).ToString(),
            };

            db.ResetPasswordCodes.Add(resetPasswordCode);

            await db.SaveChangesAsync(cancellationToken);
            var email = await emailService.SendTemplateEmail(
                           new EmailMessage
                           {
                               To = user.Email,
                               Subject = $"{resetPasswordOptions.CompanyName} || Link para alteração de senha."
                           },
                           resetPasswordOptions.MobileResetPasswordTemplateId,
                           new
                           {
                               company_name = resetPasswordOptions.CompanyName,
                               code = resetPasswordCode.Code
                           },
                           cancellationToken
                           );

            return Result.Success;
        }
    }
}
