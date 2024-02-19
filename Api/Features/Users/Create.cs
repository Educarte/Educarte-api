using Api.Infrastructure.Options;
using Api.Infrastructure.Services;
using Api.Infrastructure.Services.Email;
using Api.Infrastructure.Services.Interfaces;
using Api.Infrastructure.Validators;
using Api.Results.Users;
using Core;
using Core.Enums;
using Data;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Nudes.Retornator.Core;
using System.Text.Json.Serialization;

namespace Api.Features.Users;

/// <summary>
/// Create a new user
/// </summary>
public class Create
{

    /// <summary>
    /// Create a new user command
    /// </summary>
    public class Command : IRequest<ResultOf<UserSimpleResult>>
    {
        /// <summary>
        /// Name of user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email of user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Cellphone of user
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// Profile of user
        /// </summary>
        public Profile Profile { get; set; }
    }

    internal class Adapter : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Command, User>().Ignore(d => d.PasswordHash)
                                             .Ignore(d => d.PasswordSalt);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(ApiDbContext db)
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress().SetAsyncValidator(new UniqueUserValidator<Command>(db));

            RuleFor(x => x.Cellphone).NotEmpty().When(x => x.Profile == Profile.LegalGuardian).WithMessage("Necessário número de telefone em caso de usuário ser um responsável legal");
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<UserSimpleResult>>
    {
        private readonly ApiDbContext db;
        private readonly HashService hashService;
        private readonly IEmailService emailService;
        private readonly ResetPasswordOptions resetPasswordOptions;

        public Handler(ApiDbContext db, HashService hashService, IEmailService emailService, IOptionsSnapshot<ResetPasswordOptions> options)
        {
            this.db = db;
            this.hashService = hashService;
            this.emailService = emailService;
            this.resetPasswordOptions = options.Value;
        }

        public async Task<ResultOf<UserSimpleResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = request.Adapt<User>();
            var password = Guid.NewGuid().ToString().Substring(0, 8);

            await emailService.SendTemplateEmail(
                new EmailMessage
                {
                    To = user.Email,
                    Subject = $"{resetPasswordOptions.CompanyName} || Nova conta criada!"
                },
                resetPasswordOptions.TempPasswordTemplateId,
                new
                {
                    company_name = resetPasswordOptions.CompanyName,
                    temp_password = password,
                }, cancellationToken);

            var (passwordHash, passwordSalt) = hashService.Encrypt(password);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            db.Users.Add(user);
            await db.SaveChangesAsync(cancellationToken);

            return user.Adapt<UserSimpleResult>();
        }
    }
}
