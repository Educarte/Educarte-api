using Core.Enums;
using Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Features.Classrooms;

/// <summary>
/// Active / Inactive Student
/// </summary>
public class ToggleActive
{
    /// <summary>
    /// Active / Inactive Student
    /// </summary>
    public class Command : IRequest<Result>
    {
        /// <summary>
        /// Student id 
        /// </summary>
        public Guid Id { get; set; }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
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
            var student = await db.Students.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (student == null)
                return new NotFoundError("Estudante não encontrado.");

            student.Status = student.Status == Status.Active ? Status.Inactive : Status.Active;

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}