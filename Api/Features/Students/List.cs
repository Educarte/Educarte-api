using Api.Results.Students;
using Core.Interfaces;
using Data;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Paginator.Core;
using Nudes.Retornator.Core;

namespace Api.Features.Students;

/// <summary>
/// List all students
/// </summary>
public class List
{
    /// <summary>
    /// List all students query
    /// </summary>
    public class Query : PageRequest, IRequest<ResultOf<PageResult<StudentSimpleResult>>>
    {
        /// <summary>
        /// Filter
        /// </summary>
        public string Search { get; set; }
    }

    internal class Handler : IRequestHandler<Query, ResultOf<PageResult<StudentSimpleResult>>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<PageResult<StudentSimpleResult>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var students = db.Students
                .OnlyActives()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                students = students.Where(s => s.Name.Contains(request.Search));

            var total = await students.CountAsync(cancellationToken);

            var list = await students
                .ProjectToType<StudentSimpleResult>()
                .PaginateBy(request, s => s.Name)
                .ToListAsync(cancellationToken);

            return new PageResult<StudentSimpleResult>(request, total, list);
        }
    }
}