using Api.Results.Users;
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

namespace Api.Features.Users;

/// <summary>
/// Edit user
/// </summary>
public class Edit
{
    /// <summary>
    /// Edit user
    /// </summary>
    public class Command : IRequest<ResultOf<UserSimpleResult>>
    {
        /// <summary>
        /// Id of user
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid Id { get; set; }

        /// <summary>
        /// Name of user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email of user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Cellphone of user
        /// </summary>
        public string Cellphone { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<UserSimpleResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<UserSimpleResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await db.Users.OnlyActives().FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (user == null)
                return new NotFoundError("Usuário não encontrado.");

            if (request.Email != user.Email)
                if (await db.Users.OnlyActives().AnyAsync(x => x.Email == request.Email, cancellationToken))
                    return new BadRequestError("Email já cadastrado.");

            request.Adapt(user);

            await db.SaveChangesAsync(cancellationToken);

            return user.Adapt<UserSimpleResult>();
        }
    }
}