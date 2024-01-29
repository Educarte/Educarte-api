using Api.Results.Classroom;
using Api.Results.Menus;
using Api.Results.Students;
using Core.Enums;
using Core.Interfaces;
using Data;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Paginator.Core;
using Nudes.Retornator.Core;

namespace Api.Features.Menus;

/// <summary>
/// List all menus
/// </summary>
public class List
{
    /// <summary>
    /// List all menus query
    /// </summary>
    public class Query : PageRequest, IRequest<ResultOf<PageResult<MenuResult>>>
    {
        /// <summary>
        /// Filter
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public Status? Status { get; set; }

        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// EndDate
        /// </summary>
        public DateTime? EndDate { get; set; }
    }

    internal class Handler : IRequestHandler<Query, ResultOf<PageResult<MenuResult>>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<PageResult<MenuResult>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var menus = db.Menus
                .OnlyActives()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                menus = menus.Where(x => x.Name.Contains(request.Search));

            if (request.Status.HasValue)
                menus = menus.Where(x => x.Status == request.Status.Value);

            if (request.StartDate.HasValue)
                menus = menus.Where(x => x.StartDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                menus = menus.Where(x => x.ValidUntil <= request.EndDate.Value);

            var total = await menus.CountAsync(cancellationToken);

            var list = await menus
                .ProjectToType<MenuResult>()
                .PaginateBy(request, s => s.Name)
                .ToListAsync(cancellationToken);

            return new PageResult<MenuResult>(request, total, list);
        }
    }
}