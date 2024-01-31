﻿using Api.Infrastructure.Validators;
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
    public class Command : IRequest<ResultOf<StudentBasicResult>>
    {
        /// <summary>
        /// Student id
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid Id { get; set; }

        /// <summary>
        /// AccessControlType
        /// </summary>
        public AccessControlType AccessControlType { get; set; }
    }


    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.AccessControlType).NotNull().IsInEnum();
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<StudentBasicResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<StudentBasicResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var student = await db.Students
                .Include(x => x.AccessControls.Where(x => !x.DeletedAt.HasValue))
                .OnlyActives()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (student == null)
                return new NotFoundError("Estudante não encontrado.");

            var accessControlExists = student.AccessControls.FirstOrDefault(x => x.AccessControlType == request.AccessControlType && x.Time.Date == DateTime.Now.Date);

            if (accessControlExists != null)
                return new BadRequestError("Não é possível registrar o mesmo tipo de acesso no mesmo dia.");

            student.AccessControls.Add(new Core.AccessControl
            {
                AccessControlType = request.AccessControlType,
                StudentId = request.Id,
                Time = DateTime.Now
            });

            await db.SaveChangesAsync(cancellationToken);

            return student.Adapt<StudentBasicResult>();
        }
    }
}
