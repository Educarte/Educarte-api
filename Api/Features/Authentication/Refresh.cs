using Api.Infrastructure.Extensions.EntityExtensions;
using Api.Infrastructure.Options;
using Api.Results.Auth;
using Core.Interfaces;
using Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Features.Authentication;

/// <summary>
/// Refresh token
/// </summary>
public class Refresh
{
    /// <summary>
    /// Refresh token command
    /// </summary>
    public class Command : IRequest<ResultOf<AuthResult>>
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Token).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Command, ResultOf<AuthResult>>
    {
        private readonly ApiDbContext db;
        private readonly AuthTokenOptions authTokenOptions;

        public Handler(ApiDbContext db, IOptionsSnapshot<AuthTokenOptions> authTokenOptions)
        {
            this.db = db;
            this.authTokenOptions = authTokenOptions.Value;
        }

        public async Task<ResultOf<AuthResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var jwtToken = new JwtSecurityToken(request.Token);

            if (jwtToken.ValidTo < DateTime.Now)
                return new UnauthorizedError();

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (userId == null)
                return new UnauthorizedError();

            var user = await db.Users.IgnoreQueryFilters().OnlyActives()
                                     .FirstOrDefaultAsync(d => d.Id == Guid.Parse(userId), cancellationToken);

            if (user == null)
                return new UnauthorizedError();

            var token = user.GenerateAuthToken(authTokenOptions);

            return new AuthResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
            };
        }
    }
}
