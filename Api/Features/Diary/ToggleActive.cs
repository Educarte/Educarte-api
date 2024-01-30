using Core.Enums;
using Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Features.Diary;

/// <summary>
/// Active / Inactive
/// </summary>
public class ToggleActive
{
    /// <summary>
    /// Active / Inactive
    /// </summary>
    public class Command : IRequest<Result>
    {
        /// <summary>
        /// Id 
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
            var diary = await db.Diaries.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (diary == null)
                return new NotFoundError("Nota não encontrada.");

            diary.Status = diary.Status == Status.Active ? Status.Inactive : Status.Active;

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}