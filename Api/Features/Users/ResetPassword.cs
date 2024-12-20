﻿using Api.Infrastructure;
using Api.Infrastructure.Services;
using Api.Infrastructure.Validators;
using Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;
using System.Text.Json.Serialization;

namespace Api.Features.Users;

/// <summary>
/// Change User Password
/// </summary>
public class ResetPassword
{
    /// <summary>
    /// Reset User Password
    /// </summary>
    public class Command : IRequest<Result>
    {
        /// <summary>
        /// Id of user
        /// </summary>
        [BindNever]
        [JsonIgnore]
        public Guid UserId { get; set; }

        /// <summary>
        /// New password
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirm password
        /// </summary>
        public string ConfirmPassword { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(ApiDbContext db)
        {
            RuleFor(d => d.NewPassword).NotEmpty().SetValidator(new PasswordValidator<Command>());
        }
    }
    internal class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApiDbContext db;
        private readonly IActor actor;
        private readonly HashService hashService;

        public Handler(ApiDbContext db, IActor actor, HashService hashService)
        {
            this.db = db;
            this.actor = actor;
            this.hashService = hashService;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.ConfirmPassword != request.NewPassword)
                return new BadRequestError("Senha e Confirmação precisam ser iguais.");

            var user = await db.Users.FirstOrDefaultAsync(d => d.Id == request.UserId, cancellationToken);
            if (user == null)
                return new ForbiddenError();

            var (passwordHash, passwordSalt) = hashService.Encrypt(request.NewPassword);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            if (!user.FirstAccess.HasValue)
                user.FirstAccess = DateTime.Now;

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
