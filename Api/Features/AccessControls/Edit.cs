using Api.Infrastructure.Validators;
using Api.Results.AccessControl;
using Api.Results.Classroom;
using Api.Results.Diary;
using Api.Results.Menus;
using Core;
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

namespace Api.Features.AccessControls;

/// <summary>
/// Edit an
/// </summary>
public class Edit
{
    /// <summary>
    /// Edit and command
    /// </summary>
    public class Command : IRequest<ResultOf<AccessControlSimpleResult>>
    {
        /// <summary>
        /// Id
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid Id { get; set; }

        /// <summary>
        /// Time
        /// </summary>
        public DateTime Time { get; set; }
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
        public Validator()
        {
            RuleFor(x => x.Time).NotEmpty();
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
            var accessControl = await db.AccessControls
                .OnlyActives()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            request.Adapt(accessControl);

            await db.SaveChangesAsync(cancellationToken);

            return accessControl.Adapt<AccessControlSimpleResult>();
        }
    }
}
