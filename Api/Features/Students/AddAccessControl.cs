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
/// Add access control
/// </summary>
public class AddAccessControl
{
    /// <summary>
    /// Add access control command
    /// </summary>
    public class Command : IRequest<ResultOf<MessageResult>>
    {
        /// <summary>
        /// Student id
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid Id { get; set; }
    }


    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
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
                .Include(x => x.AccessControls.Where(x => !x.DeletedAt.HasValue))
                .OnlyActives()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (student == null)
                return new NotFoundError("Estudante não encontrado.");

            var accessControlExists = student.AccessControls.Where(x => x.Time.Date == DateTime.Now.Date);

            if (accessControlExists.Where(x => x.AccessControlType == AccessControlType.Entrance || x.AccessControlType == AccessControlType.Exit).Count() > 1)
                return new BadRequestError("Este usuário já registrou entrada e saída para o dia de hoje.");

            student.AccessControls.Add(new Core.AccessControl
            {
                AccessControlType = accessControlExists.FirstOrDefault(x => x.AccessControlType == AccessControlType.Entrance) == null ? AccessControlType.Entrance : AccessControlType.Exit,
                StudentId = request.Id,
                Time = DateTime.UtcNow
            });

            await db.SaveChangesAsync(cancellationToken);

            return new MessageResult($"Acesso do aluno registrado.");
        }
    }
}
