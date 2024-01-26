using Api.Results.Classroom;
using Api.Results.Students;
using Core.Enums;
using Core.Interfaces;
using Data;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Paginator.Core;
using Nudes.Retornator.Core;

namespace Api.Features.Classrooms;

/// <summary>
/// List all classrooms
/// </summary>
public class List
{
    /// <summary>
    /// List all classrooms query
    /// </summary>
    public class Query : PageRequest, IRequest<ResultOf<PageResult<ClassroomSimpleResult>>>
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
        /// Time
        /// </summary>
        public Time? Time { get; set; }

        /// <summary>
        /// ClassroomType
        /// </summary>
        public ClassroomType? ClassroomType { get; set; }
    }

    internal class Handler : IRequestHandler<Query, ResultOf<PageResult<ClassroomSimpleResult>>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<PageResult<ClassroomSimpleResult>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var classrooms = db.Classrooms
                .Include(x => x.Students)
                .OnlyActives()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                classrooms = classrooms.Where(x => x.Name.Contains(request.Search));

            if (request.Status.HasValue)
                classrooms = classrooms.Where(x => x.Status == request.Status.Value);

            if (request.Status.HasValue)
                classrooms = classrooms.Where(x => x.ClassroomType == request.ClassroomType.Value);

            if (request.Status.HasValue)
                classrooms = classrooms.Where(x => x.Time == request.Time.Value);

            if (request.Status.HasValue)
                classrooms = classrooms.Where(x => x.Status == request.Status.Value);

            var total = await classrooms.CountAsync(cancellationToken);

            var list = await classrooms
                .ProjectToType<ClassroomSimpleResult>()
                .PaginateBy(request, s => s.Name)
                .ToListAsync(cancellationToken);

            return new PageResult<ClassroomSimpleResult>(request, total, list);
        }
    }
}