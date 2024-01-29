﻿using Api.Infrastructure.Validators;
using Api.Results.Classroom;
using Api.Results.Diary;
using Api.Results.Menus;
using Core;
using Core.Enums;
using Core.Interfaces;
using Data;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;
using System.Text.Json.Serialization;

namespace Api.Features.Diary;

/// <summary>
/// Edit an
/// </summary>
public class Edit
{
    /// <summary>
    /// Edit and command
    /// </summary>
    public class Command : IRequest<ResultOf<DiarySimpleResult>>
    {
        /// <summary>
        /// Id
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid Id { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// FileUri
        /// </summary>
        public string FileUri { get; set; }

        /// <summary>
        /// IsDiaryForAll
        /// </summary>
        public bool IsDiaryForAll { get; set; }

        /// <summary>
        /// Time
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// StudentIds
        /// </summary>
        public IList<Guid> StudentIds { get; set; }

        /// <summary>
        /// ClassroomIds
        /// </summary>
        public IList<Guid> ClassroomIds { get; set; }
    }

    internal class Adapter : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Command, Core.Diary>();
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator(ApiDbContext db)
        {
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.FileUri).NotEmpty();
            RuleFor(x => x.IsDiaryForAll).NotEmpty();
            RuleFor(x => x.Time).NotEmpty();

            When(x => !x.IsDiaryForAll && x.StudentIds.Any(), () =>
            {
                RuleForEach(x => x.StudentIds).NotEmpty().SetAsyncValidator(new StudentExistenceValidator<Command>(db)).When(x => !x.ClassroomIds.Any()).WithMessage("Estudante não existe ou lista de salas vieram preenchidas.");
            });

            When(x => !x.IsDiaryForAll && x.ClassroomIds.Any(), () =>
            {
                RuleForEach(x => x.ClassroomIds).NotEmpty().SetAsyncValidator(new ClassroomExistenceValidator<Command>(db)).When(x => !x.StudentIds.Any()).WithMessage("Turma não existe ou lista de estudantes vieram preenchidas.");
            });
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<DiarySimpleResult>>
    {
        private readonly ApiDbContext db;

        public Handler(ApiDbContext db)
        {
            this.db = db;
        }

        public async Task<ResultOf<DiarySimpleResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var diary = await db.Diaries
                .Include(x => x.User)
                .Include(x => x.Students)
                .Include(x => x.Classrooms)
                .OnlyActives()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (diary == null)
                return new NotFoundError("Nota não encontrada.");

            request.Adapt(diary);

            if (!request.IsDiaryForAll && request.StudentIds.Any())
            {
                var studentsToAdd = request.StudentIds.Except(diary.Students.Select(x => x.Id))
                              .Select(studentId => new Student { Id = studentId }).ToList();
                db.Students.AttachRange(studentsToAdd);

                diary.Students.AddRange(studentsToAdd);
                diary.Students.RemoveAll(diary.Students.Where(m => !request.StudentIds.Contains(m.Id)).Contains);
            }
            else if (!request.IsDiaryForAll && request.ClassroomIds.Any())
            {
                var classroomsToAdd = request.ClassroomIds.Except(diary.Classrooms.Select(x => x.Id))
                                  .Select(classroomId => new Classroom { Id = classroomId }).ToList();
                db.Classrooms.AttachRange(classroomsToAdd);

                diary.Classrooms.AddRange(classroomsToAdd);
                diary.Classrooms.RemoveAll(diary.Classrooms.Where(m => !request.ClassroomIds.Contains(m.Id)).Contains);
            }


            await db.SaveChangesAsync(cancellationToken);

            return diary.Adapt<DiarySimpleResult>();
        }
    }
}
