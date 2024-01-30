using Api.Infrastructure.Validators;
using Api.Results.Students;
using Core.Enums;
using Core.Interfaces;
using Data;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;
using System.Text.Json.Serialization;

namespace Api.Features.Students;

/// <summary>
/// Edit an student
/// </summary>
public class Edit
{
    /// <summary>
    /// Edit and student command
    /// </summary>
    public class Command : IRequest<ResultOf<StudentResult>>
    {
        /// <summary>
        /// Student id
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid Id { get; set; }

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
            /// Legal guardian id
            /// </summary>
            public Guid Id { get; set; }

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
            /// Address id
            /// </summary>
            public Guid Id { get; set; }

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
            /// Contracted hour id
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// EndDate
            /// </summary>
            public DateTime? EndDate { get; set; }
        }

        /// <summary>
        /// Emergency contact Command
        /// </summary>
        public class EmergencyContactCommand
        {
            /// <summary>
            /// Emergency contact id
            /// </summary>
            public Guid Id { get; set; }

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


    public class Validator : AbstractValidator<Command>
    {
        public Validator(LegalGuardianExistenceValidator<Command> legalGuardianExistenceValidator,
            ClassroomExistenceValidator<Command> classroomExistenceValidator)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.LegalGuardians).NotEmpty();
            RuleFor(x => x.ClassroomId).NotEmpty().SetAsyncValidator(classroomExistenceValidator);
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<StudentResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<StudentResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var student = await db.Students.OnlyActives().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (student == null)
                return new NotFoundError("Estudante não encontrado.");

            request.Adapt(student);

            await db.SaveChangesAsync(cancellationToken);

            return student.Adapt<StudentResult>();
        }
    }
}
