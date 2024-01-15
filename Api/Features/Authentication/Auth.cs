using Api.Infrastructure.Extensions.EntityExtensions;
using Api.Infrastructure.Options;
using Api.Infrastructure.Services;
using Api.Results.Auth;
using Core.Enums;
using Core.Interfaces;
using Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Features.Authentication
{
    /// <summary>
    /// Auth user
    /// </summary>
    public class Auth
    {
        /// <summary>
        /// Auth user command
        /// </summary>
        public class Command : IRequest<ResultOf<AuthResult>>
        {
            /// <summary>
            /// Email of user
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// Password of user
            /// </summary>
            public string Password { get; set; }
        }

        internal class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(d => d.Email).NotEmpty().EmailAddress();
                RuleFor(d => d.Password).NotEmpty().MinimumLength(8);
            }
        }

        internal class Handler : IRequestHandler<Command, ResultOf<AuthResult>>
        {
            private readonly ApiDbContext db;
            private readonly HashService hashService;
            private readonly AuthTokenOptions tokenOptions;

            public Handler(ApiDbContext db, IOptionsSnapshot<AuthTokenOptions> tokenOptions, HashService hashService)
            {
                this.db = db;
                this.hashService = hashService;
                this.tokenOptions = tokenOptions.Value;
            }

            public async Task<ResultOf<AuthResult>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await db.Users.OnlyActives().FirstOrDefaultAsync(d => d.Email == request.Email && d.Status == Status.Active, cancellationToken);

                if (user == null || !hashService.Compare(user.PasswordHash, user.PasswordSalt, request.Password))
                    return new UnauthorizedError();

                var token = user.GenerateAuthToken(tokenOptions);

                return new AuthResult
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                };
            }
        }
    }
}