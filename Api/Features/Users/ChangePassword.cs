﻿using Api.Infrastructure;
using Api.Infrastructure.Services;
using Api.Infrastructure.Validators;
using Api.Results.Users;
using Data;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;

namespace Api.Features.Users;

/// <summary>
/// Change User Password
/// </summary>
public class ChangePassword
{
    /// <summary>
    /// Change User Password
    /// </summary>
    public class Command : IRequest<ResultOf<UserSimpleResult>>
    {
        /// <summary>
        /// Current password
        /// </summary>
        public string CurrentPassword { get; set; }

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
        public Validator()
        {
            RuleFor(d => d.NewPassword).NotEmpty().SetValidator(new PasswordValidator<Command>());
        }
    }
    internal class Handler : IRequestHandler<Command, ResultOf<UserSimpleResult>>
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

        public async Task<ResultOf<UserSimpleResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.ConfirmPassword != request.NewPassword)
                return new BadRequestError("Senha e Confirmação precisam ser iguais.");

            var user = await db.Users.FirstOrDefaultAsync(d => d.Id == actor.UserId, cancellationToken);
            if (user == null)
                return new ForbiddenError();

            if (!hashService.Compare(user.PasswordHash, user.PasswordSalt, request.CurrentPassword))
                return new ForbiddenError();

            var (passwordHash, passwordSalt) = hashService.Encrypt(request.NewPassword);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await db.SaveChangesAsync(cancellationToken);

            return user.Adapt<UserSimpleResult>();
        }
    }
}
