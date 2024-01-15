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
/// List Users
/// </summary>
public class List
{
    /// <summary>
    /// List Users
    /// </summary>
    public class Query : PageRequest, IRequest<ResultOf<PageResult<UserResult>>>
    {
        /// <summary>
        /// Filter by name and email
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Filter by users actives or inactives
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Filter by user profile
        /// </summary>
        public Profile? Profile { get; set; }
    }

    internal class Handler : IRequestHandler<Query, ResultOf<PageResult<UserResult>>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<PageResult<UserResult>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var users = db.Users.OnlyActives().Where(x => x.Profile != Profile.Admin);

            if (!string.IsNullOrWhiteSpace(request.Search))
                users = users.Where(u => u.Name.Contains(request.Search) || u.Email.Contains(request.Search));

            if (request.IsActive.HasValue)
                users = users.Where(ser => request.IsActive.Value ? ser.Status == Status.Active : ser.Status == Status.Inactive);

            if (request.Profile.HasValue)
                users = users.Where(x => x.Profile == request.Profile.Value);

            var total = await users.CountAsync(cancellationToken);

            var list = await users
                .ProjectToType<UserResult>()
                .PaginateBy(request, s => s.Name)
                .ToListAsync(cancellationToken);

            return new PageResult<UserResult>(request, total, list);
        }
    }
}