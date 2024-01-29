using Api.Infrastructure.Validators;
using Data;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.Core;
using Api.Results.Diary;
using Api.Infrastructure;
using Core.Enums;
using Api.Results.AccessControl;

namespace Api.Features.AccessControls;

/// <summary>
/// Create
/// </summary>
public class Create
{
    /// <summary>
    /// Create command
    /// </summary>
    public class Command : IRequest<ResultOf<AccessControlSimpleResult>>
    {
        /// <summary>
        /// AccessControlType
        /// </summary>
        public AccessControlType AccessControlType { get; set; }

        /// <summary>
        /// StudentId
        /// </summary>
        public Guid StudentId { get; set; }
    }

    internal class Adapter : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Command, Core.AccessControl>();
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator(ApiDbContext db)
        {
            RuleFor(x => x.AccessControlType).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty().SetAsyncValidator(new StudentExistenceValidator<Command>(db));
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<AccessControlSimpleResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<AccessControlSimpleResult>> Handle(Command request, CancellationToken cancellationToken)
        {

            var accessControl = request.Adapt<Core.AccessControl>();

            accessControl.Time = DateTime.UtcNow;

            db.AccessControls.Add(accessControl);
            await db.SaveChangesAsync(cancellationToken);

            return accessControl.Adapt<AccessControlSimpleResult>();
        }
    }
}
