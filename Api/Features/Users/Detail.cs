using Api.Results.Users;
using Core.Interfaces;
using Data;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Features.Users;

/// <summary>
/// Find user by id
/// </summary>
public class Detail
{
    /// <summary>
    /// Find user by id
    /// </summary>
    public class Query : IRequest<ResultOf<UserSimpleResult>>
    {
        /// <summary>
        /// user id 
        /// </summary>
        public Guid Id { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Query, ResultOf<UserSimpleResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<UserSimpleResult>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await db.Users.OnlyActives().FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
            if (user == null)
                return new NotFoundError("Usuário não encontrado.");

            return user.Adapt<UserSimpleResult>();
        }
    }
}
