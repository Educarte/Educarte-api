using Api.Results.Diary;
using Api.Results.Generic;
using Core.Enums;
using Core.Interfaces;
using Data;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Paginator.Core;
using Nudes.Retornator.Core;

namespace Api.Features.Diary;

/// <summary>
/// ListMobile
/// </summary>
public class ListMobile
{
    /// <summary>
    /// ListMobile
    /// </summary>
    public class Query : PageRequest, IRequest<ResultOf<MobileListResult<DiaryResult>>>
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

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

        /// <summary>
        /// StudentId
        /// </summary>
        public Guid? StudentId { get; set; }

        /// <summary>
        /// ClassroomId
        /// </summary>
        public Guid? ClassroomId { get; set; }
    }

    internal class Handler : IRequestHandler<Query, ResultOf<MobileListResult<DiaryResult>>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<MobileListResult<DiaryResult>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var diaries = db.Diaries
                .OnlyActives()
                .AsQueryable();

            if (request.Status.HasValue)
                diaries = diaries.Where(x => x.Status == request.Status.Value);

            if (request.StudentId.HasValue && request.ClassroomId.HasValue)
                diaries = diaries.Where(x => (x.Students.Any(y => y.Id == request.StudentId) || x.Classrooms.Any(y => y.Id == request.ClassroomId)) || x.IsDiaryForAll);

            if (request.StudentId.HasValue && !request.ClassroomId.HasValue)
                diaries = diaries.Where(x => x.Students.Any(y => y.Id == request.StudentId) || x.IsDiaryForAll);

            if (request.ClassroomId.HasValue && !request.StudentId.HasValue)
                diaries = diaries.Where(x => x.Classrooms.Any(y => y.Id == request.ClassroomId));

            if (request.StartDate.HasValue)
                diaries = diaries.Where(x => x.CreatedAt.Date >= request.StartDate.Value.Date);

            if (request.EndDate.HasValue)
                diaries = diaries.Where(x => x.CreatedAt.Date <= request.EndDate.Value.Date);

            if (!string.IsNullOrEmpty(request.Name))
                diaries = diaries.Where(x => x.Name.Contains(request.Name));

            var list = await diaries
                .ProjectToType<DiaryResult>()
                .PaginateBy(request, d => d.Name)
                .ToListAsync(cancellationToken);

            return new MobileListResult<DiaryResult>(list);
        }
    }
}
