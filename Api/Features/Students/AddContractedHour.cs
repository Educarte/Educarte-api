using Api.Infrastructure.Validators;
using Api.Results.Generic;
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
/// Add Contracted Hour
/// </summary>
public class AddContractedHour
{
    /// <summary>
    /// Add Contracted Hour command
    /// </summary>
    public class Command : IRequest<ResultOf<MessageResult>>
    {
        /// <summary>
        /// Student id
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid Id { get; set; }

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


    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Hours).GreaterThan(0);
            RuleFor(x => x.StartDate).NotEmpty().GreaterThanOrEqualTo(DateTime.Now.Date);
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate.Date).When(x => x.EndDate.HasValue).WithMessage("A data de término do contrato deve ser maior que a data de início.");
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<MessageResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<MessageResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var student = await db.Students
                .Include(x => x.ContractedHours.Where(x => !x.DeletedAt.HasValue))
                .OnlyActives()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (student == null)
                return new NotFoundError("Estudante não encontrado.");

            var contractedHourExists = student.ContractedHours.FirstOrDefault(x => (x.EndDate.HasValue ? x.EndDate.Value.Date >= DateTime.Now.Date : true));

            if (contractedHourExists != null)
                return new BadRequestError("Para adicionar um novo contrato, é necessário definir a data de término do contrato atual.");

            var contractedHourActive = student.ContractedHours.FirstOrDefault(x => x.Status == Status.Active);

            if (request.StartDate.Date <= contractedHourActive.StartDate.Date)
                return new BadRequestError("A data de início do novo contrato deve ser maior que a data de início do contrato anterior.");

            contractedHourActive.Status = Status.Inactive;

            student.ContractedHours.Add(new Core.ContractedHour
            {
                Hours = request.Hours,
                StartDate = request.StartDate.Date,
                EndDate = request.EndDate.HasValue ? request.EndDate.Value.Date : null,
            });

            await db.SaveChangesAsync(cancellationToken);

            return new MessageResult("Contrato de horas do aluno atualizada.");
        }
    }
}
