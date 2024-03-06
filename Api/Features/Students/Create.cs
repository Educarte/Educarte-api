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

namespace Api.Features.Students;

/// <summary>
/// Create a student
/// </summary>
public class Create
{
    /// <summary>
    /// Create a student command
    /// </summary>
    public class Command : IRequest<ResultOf<StudentResult>>
    {
        /// <summary>
        /// Name of student
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Naturalness
        /// </summary>
        public string Naturalness { get; set; }

        /// <summary>
        /// HealthProblem
        /// </summary>
        public string HealthProblem { get; set; }

        /// <summary>
        /// AllergicFood
        /// </summary>
        public string AllergicFood { get; set; }

        /// <summary>
        /// AllergicMedicine
        /// </summary>
        public string AllergicMedicine { get; set; }

        /// <summary>
        /// Epilepsy
        /// </summary>
        public bool Epilepsy { get; set; }

        /// <summary>
        /// AllergicBugBite
        /// </summary>
        public string AllergicBugBite { get; set; }

        /// <summary>
        /// SpecialChild
        /// </summary>
        public string SpecialChild { get; set; }

        /// <summary>
        /// SpecialChildHasReport
        /// </summary>
        public bool SpecialChildHasReport { get; set; }

        /// <summary>
        /// BirthDate
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Genre
        /// </summary>
        public Genre Genre { get; set; }

        /// <summary>
        /// BloodType
        /// </summary>
        public BloodType BloodType { get; set; }

        /// <summary>
        /// Morning or night shift
        /// </summary>
        public Time Time { get; set; }

        /// <summary>
        /// Classroom Id
        /// </summary>
        public Guid ClassroomId { get; set; }

        /// <summary>
        /// Contracted hours
        /// </summary>
        public IList<ContractedHourCommand> ContractedHours { get; set; }

        /// <summary>
        /// Contracted hours
        /// </summary>
        public IList<EmergencyContactCommand> EmergencyContacts { get; set; }

        /// <summary>
        /// Legal Guardians
        /// </summary>
        public IList<LegalGuardianCommand> LegalGuardians { get; set; }

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

        /// <summary>
        /// Contracted Hour Command
        /// </summary>
        public class ContractedHourCommand
        {
            /// <summary>
            /// Hours
            /// </summary>
            public decimal Hours { get; set; }

            /// <summary>
            /// StartDate
            /// </summary>
            public DateTime StartDate { get; set; }

            /// <summary>
            /// EndDate
            /// </summary>
            public DateTime? EndDate { get; set; }
        }

        /// <summary>
        /// Contracted Hour Command
        /// </summary>
        public class EmergencyContactCommand
        {
            /// <summary>
            /// Hours
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Telephone
            /// </summary>
            public string Telephone { get; set; }
        }
    }

    internal class Adapter : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Command, Student>();

            config.NewConfig<Command.LegalGuardianCommand, User>()
                .Map(x => x.Profile, x => Profile.LegalGuardian);

            config.NewConfig<Command.ContractedHourCommand, ContractedHour>();
            config.NewConfig<Command.EmergencyContactCommand, EmergencyContact>();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(ApiDbContext db)
        {
            RuleFor(x => x.Name).NotEmpty();

            RuleForEach(x => x.EmergencyContacts).ChildRules(x =>
            {
                x.RuleFor(x => x.Name).NotEmpty();
                x.RuleFor(x => x.Telephone).NotEmpty();
            }).NotEmpty();

            RuleForEach(x => x.ContractedHours).ChildRules(x =>
            {
                x.RuleFor(x => x.Hours).GreaterThan(0);
                x.RuleFor(x => x.StartDate).NotEmpty().GreaterThanOrEqualTo(DateTime.Now.Date);
                x.RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate.Date).When(x => x.EndDate.HasValue).WithMessage("A data de término do contrato deve ser maior que a data de início.");
            }).NotEmpty();

            RuleForEach(x => x.LegalGuardians)
                .ChildRules(x =>
                {
                    x.RuleFor(x => x.Cellphone).NotEmpty();
                    x.RuleFor(x => x.Name).NotEmpty();
                    x.RuleFor(x => x.Email).NotEmpty().EmailAddress().SetAsyncValidator(new UniqueUserValidator<Command.LegalGuardianCommand>(db));
                    x.RuleFor(x => x.LegalGuardianType).NotEmpty();
                    x.RuleFor(x => x.Address).NotEmpty();
                }).NotEmpty();

            RuleFor(x => x.ClassroomId).NotEmpty().SetAsyncValidator(new ClassroomExistenceValidator<Command>(db)).WithMessage("Turma não existe");
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
            var legalGuardians = await db.Users
                .Include(x => x.Childs)
                .OnlyActives()
                .Where(x => request.LegalGuardians.Select(x => x.Email).Contains(x.Email)).ToListAsync(cancellationToken);

            var legalGuardiansToAddEmails = request.LegalGuardians.Select(x => x.Email).Except(legalGuardians.Select(x => x.Email));
            var legalGuardiansToAdd = request.LegalGuardians.Where(x => legalGuardiansToAddEmails.Contains(x.Email)).ToList();
            var legalGuardiansToBind = legalGuardians.Where(x => !legalGuardiansToAddEmails.Contains(x.Email)).ToList();

            if (legalGuardiansToBind.Select(x => x.Childs).Any(x => x.Count >= 2))
                return new BadRequestError("Responsável legal selecionado já contem 2 crianças vinculadas ao mesmo.");

            var student = request.Adapt<Student>();
            var users = legalGuardiansToAdd.Select(x => x.Adapt<User>()).ToList();

            foreach (var user in users)
            {
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
            }

            if(legalGuardiansToBind.Any())
                users.AddRange(legalGuardiansToBind);

            student.LegalGuardians = users;

            db.Students.Add(student);
            await db.SaveChangesAsync(cancellationToken);

            return student.Adapt<StudentResult>();
        }
    }
}
