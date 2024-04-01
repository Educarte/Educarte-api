using Api.Results.Users;
using Core.Enums;
using Core.Interfaces;
using Data;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Paginator.Core;
using Nudes.Retornator.Core;

namespace Api.Features.Users;

/// <summary>
/// Get User
/// </summary>
public class Get
{
    /// <summary>
    /// Get User
    /// </summary>
    public class Query : PageRequest, IRequest<ResultOf<UserSimpleResult>>
    {
        /// <summary>
        /// Filter by name and email
        /// </summary>
        public string Search { get; set; }
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
            var user = await db.Users
                .Include(x => x.Address)
                .OnlyActives()
                .FirstOrDefaultAsync(x => x.Profile != Profile.Admin && (x.Name.Contains(request.Search) || x.Email.Contains(request.Search)), cancellationToken);

            return user.Adapt<UserSimpleResult>();
        }
    }
}