﻿using Api.Results.Classroom;
using Api.Results.Diary;
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

namespace Api.Features.Diary;

/// <summary>
/// List all
/// </summary>
public class List
{
    /// <summary>
    /// List all query
    /// </summary>
    public class Query : PageRequest, IRequest<ResultOf<PageResult<DiaryResult>>>
    {
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

    internal class Handler : IRequestHandler<Query, ResultOf<PageResult<DiaryResult>>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<PageResult<DiaryResult>>> Handle(Query request, CancellationToken cancellationToken)
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

            var total = await diaries.CountAsync(cancellationToken);

            var list = await diaries
                .ProjectToType<DiaryResult>()
                .ToListAsync(cancellationToken);

            return new PageResult<DiaryResult>(request, total, list);
        }
    }
}