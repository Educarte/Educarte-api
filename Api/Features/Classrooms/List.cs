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
    public class Query : PageRequest, IRequest<ResultOf<PageResult<ClassroomStudentsSimpleResult>>>
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

    internal class Handler : IRequestHandler<Query, ResultOf<PageResult<ClassroomStudentsSimpleResult>>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<PageResult<ClassroomStudentsSimpleResult>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var classrooms = db.Classrooms
                .Include(x => x.Students)
                .Include(x => x.Teachers)
                .OnlyActives()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                classrooms = classrooms.Where(x => x.Name.Contains(request.Search));

            if (request.Status.HasValue)
                classrooms = classrooms.Where(x => x.Status == request.Status.Value);

            if (request.ClassroomType.HasValue)
                classrooms = classrooms.Where(x => x.ClassroomType == request.ClassroomType.Value);

            if (request.Time.HasValue)
                classrooms = classrooms.Where(x => x.Time == request.Time.Value);

            var total = await classrooms.CountAsync(cancellationToken);

            var list = await classrooms
                .ProjectToType<ClassroomStudentsSimpleResult>()
                .PaginateBy(request, s => s.Name)
                .ToListAsync(cancellationToken);

            return new PageResult<ClassroomStudentsSimpleResult>(request, total, list);
        }
    }
}