﻿using Api.Infrastructure.Validators;
using Api.Results.Students;
using Core.Enums;
using Core.Interfaces;
using Data;
using FluentValidation;
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

        /// <summary>
        /// StudentStatus
        /// </summary>
        public Status? StudentStatus { get; set; }
        
        /// <summary>
        /// ContractStatus
        /// </summary>
        public Status? ContractStatus { get; set; }

        /// <summary>
        /// ClassroomType
        /// </summary>
        public ClassroomType? ClassroomType { get; set; }

        /// <summary>
        /// LegalGuardianId
        /// </summary>
        public Guid? LegalGuardianId { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator(ApiDbContext db)
        {
            RuleFor(x => x.LegalGuardianId).NotEmpty().SetAsyncValidator(new LegalGuardianExistenceValidator<Query>(db)).When(x => x.LegalGuardianId.HasValue);
        }
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
                .Include(x => x.LegalGuardians)
                .Include(x => x.ContractedHours)
                .Include(x => x.Classroom)
                .Include(x => x.AccessControls)
                .OnlyActives()
                .AsQueryable();

            if (request.LegalGuardianId.HasValue)
                students = students.Where(x => x.LegalGuardians.Any(y => y.Id == request.LegalGuardianId.Value));

            if (!string.IsNullOrWhiteSpace(request.Search))
                students = students.Where(s => s.Name.Contains(request.Search));

            if (request.StudentStatus.HasValue)
                students = students.Where(x => x.Status == request.StudentStatus.Value);

            if (request.ContractStatus.HasValue)
                students = students.Where(x => x.ContractedHours.Any(x => x.Status == request.ContractStatus.Value));

            if (request.ClassroomType.HasValue)
                students = students.Where(x => x.Classroom.ClassroomType == request.ClassroomType);

            var total = await students.CountAsync(cancellationToken);

            var list = await students
                .ProjectToType<StudentSimpleResult>()
                .PaginateBy(request, s => s.Name)
                .ToListAsync(cancellationToken);

            return new PageResult<StudentSimpleResult>(request, total, list);
        }
    }
}