using Core;
using Core.Interfaces;
using Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Features.Classrooms;

/// <summary>
/// Delete an classroom
/// </summary>
public class Delete
{
    /// <summary>
    /// Delete and classroom command
    /// </summary>
    public class Command : IRequest<Result>
    {
        /// <summary>
        /// Classroom Id
        /// </summary>
        public Guid Id { get; set; }
    }

    public class Validator : AbstractValidator<Command>
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
            var classroom = await db.Classrooms
                .Include(x => x.Students)
                .OnlyActives()
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (classroom == null)
                return new NotFoundError("Turma não encontrada.");

            if (classroom.Students.Any())
                classroom.Students.RemoveAll(classroom.Students.Contains);

            classroom.DeletedAt = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}