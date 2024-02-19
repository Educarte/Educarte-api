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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Api.Features.Students;

/// <summary>
/// Add Legal Guardian
/// </summary>
public class AddLegalGuardian
{
    /// <summary>
    /// Add Legal Guardian command
    /// </summary>
    public class Command : IRequest<ResultOf<StudentResult>>
    {
        /// <summary>
        /// StudentId
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid StudentId { get; set; }

        /// <summary>
        /// Legal Guardians
        /// </summary>
        public LegalGuardianCommand LegalGuardian { get; set; }

        /// <summary>
        /// Legal guardian command
        /// </summary>
        public class LegalGuardianCommand
        {
            /// <summary>
            /// Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Email
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// Cellphone
            /// </summary>
            public string Cellphone { get; set; }

            /// <summary>
            /// LegalGuardianType
            /// </summary>
            public string LegalGuardianType { get; set; }

            /// <summary>
            /// Address
            /// </summary>
            public AddressCommand Address { get; set; }
        }

        /// <summary>
        /// Address command
        /// </summary>
        public class AddressCommand
        {
            /// <summary>
            /// Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Cep
            /// </summary>
            public string Cep { get; set; }

            /// <summary>
            /// Street
            /// </summary>
            public string Street { get; set; }

            /// <summary>
            /// Number
            /// </summary>
            public string Number { get; set; }

            /// <summary>
            /// District
            /// </summary>
            public string District { get; set; }

            /// <summary>
            /// Complement
            /// </summary>
            public string Complement { get; set; }

            /// <summary>
            /// Reference
            /// </summary>
            public string Reference { get; set; }

            /// <summary>
            /// Telephone
            /// </summary>
            public string Telephone { get; set; }
        }
    }

    public class Adapter : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Command.LegalGuardianCommand, User>()
                .Map(x => x.Profile, x => Profile.LegalGuardian);

            config.NewConfig<Command.AddressCommand, Core.Address>();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(ApiDbContext db)
        {
            RuleFor(x => x.StudentId).NotEmpty().SetAsyncValidator(new StudentExistenceValidator<Command>(db));
            RuleFor(x => x.LegalGuardian.Cellphone).NotEmpty();
            RuleFor(x => x.LegalGuardian.Name).NotEmpty();
            RuleFor(x => x.LegalGuardian.Email).NotEmpty().EmailAddress().SetAsyncValidator(new UniqueUserValidator<Command>(db));
            RuleFor(x => x.LegalGuardian.LegalGuardianType).NotEmpty();
            RuleFor(x => x.LegalGuardian.Address).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<StudentResult>>
    {
        private readonly ApiDbContext db;
        private readonly HashService hashService;
        private readonly IEmailService emailService;
        private readonly ResetPasswordOptions resetPasswordOptions;

        public Handler(ApiDbContext db, HashService hashService, IEmailService emailService, IOptionsSnapshot<ResetPasswordOptions> resetPasswordOptions)
        {
            this.db = db;
            this.hashService = hashService;
            this.emailService = emailService;
            this.resetPasswordOptions = resetPasswordOptions.Value;
        }

        public async Task<ResultOf<StudentResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var student = await db.Students
                .Include(x => x.LegalGuardians)
                .OnlyActives()
                .FirstOrDefaultAsync(x => x.Id == request.StudentId, cancellationToken);

            if (student == null)
                return new NotFoundError("Estudante não foi encontrado");

            var user = request.LegalGuardian.Adapt<User>();

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

            student.LegalGuardians.Add(user);

            await db.SaveChangesAsync(cancellationToken);

            return student.Adapt<StudentResult>();
        }
    }
}
