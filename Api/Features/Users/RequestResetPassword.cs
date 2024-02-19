using Api.Infrastructure.Options;
using Api.Infrastructure.Services.Email;
using Api.Infrastructure.Services.Interfaces;
using Core;
using Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nudes.Retornator.Core;

namespace Api.Features.Users;

/// <summary>
/// Request reset password
/// </summary>
public class RequestResetPassword
{
    /// <summary>
    /// Reques reset password command
    /// </summary>
    public class Command : IRequest<Result>
    {
        /// <summary>
        /// Email of user
        /// </summary>
        public string Email { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(d => d.Email).NotEmpty().EmailAddress();
        }
    }

    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApiDbContext db;
        private readonly IEmailService emailService;
        private readonly ResetPasswordOptions resetPasswordOptions;

        public Handler(ApiDbContext db, IEmailService emailService, IOptionsSnapshot<ResetPasswordOptions> options)
        {
            this.db = db;
            this.emailService = emailService;
            this.resetPasswordOptions = options.Value;
        }
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await db.Users.FirstOrDefaultAsync(d => d.Email == request.Email, cancellationToken);
            if (user == null)
                return Result.Success;

            var resetPasswordCode = new ResetPasswordCode
            {
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddMinutes(resetPasswordOptions.CodeExpirationTime),
                Code = Guid.NewGuid().ToString(),
            };

            db.ResetPasswordCodes.Add(resetPasswordCode);

            await db.SaveChangesAsync(cancellationToken);
            var email = await emailService.SendTemplateEmail(
                new EmailMessage
                {
                    To = user.Email,
                    Subject = $"{resetPasswordOptions.CompanyName} || Link para alteração de senha."
                },
                resetPasswordOptions.DesktopResetPasswordTemplateId,
                new
                {
                    company_name = resetPasswordOptions.CompanyName,
                    reset_password_url = new Uri($"{resetPasswordOptions.ResetUri}/{resetPasswordCode.Code}")
                },
                cancellationToken
                );
            return Result.Success;
        }
    }
}
