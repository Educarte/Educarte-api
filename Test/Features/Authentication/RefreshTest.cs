using Api.Features.Authentication.Methods;
using Api.Features.Users;
using Api.Infrastructure.Extensions.EntityExtensions;
using Api.Infrastructure.Options;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nudes.Retornator.AspnetCore.Errors;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Test.Features.Authentication
{
    public class RefreshTest : BaseTest
    {
        private readonly AuthTokenOptions authTokenOptions;

        public RefreshTest(ServerFixture serverFixture) : base(serverFixture)
        {
            authTokenOptions = serverFixture.ServiceProvider.GetService<IOptionsSnapshot<AuthTokenOptions>>().Value;
        }


        [Fact]
        public async Task Should_Refresh_Valid_Token()
        {
            var createUserCommand = new Faker<Create.Command>()
                                            .RuleFor(d => d.FirstName, d => d.Person.FirstName)
                                            .RuleFor(d => d.LastName, d => d.Person.LastName)
                                            .RuleFor(d => d.Email, d => d.Internet.Email())
                                            .RuleFor(d => d.Password, d => d.Internet.Password())
                                            .Generate();

            var createResult = await mediator.Send(createUserCommand);

            var user = await db.Users.FirstOrDefaultAsync(d => d.Id == createResult.Result.Id);

            var token = user.GenerateAuthToken(authTokenOptions);

            var refreshResult = await mediator.Send(new Refresh.Command
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            });
            refreshResult.Should().NotBeNull();
            refreshResult.Error.Should().BeNull();

            refreshResult.Result.Should().NotBeNull();
            refreshResult.Result.Token.Should().NotBeNull();
        }

        [Fact]
        public async Task Should_Unauthorize_Expired_Token()
        {
            var createUserCommand = new Faker<Create.Command>()
                                            .RuleFor(d => d.FirstName, d => d.Person.FirstName)
                                            .RuleFor(d => d.LastName, d => d.Person.LastName)
                                            .RuleFor(d => d.Email, d => d.Internet.Email())
                                            .RuleFor(d => d.Password, d => d.Internet.Password())
                                            .Generate();

            var createResult = await mediator.Send(createUserCommand);

            var user = await db.Users.FirstOrDefaultAsync(d => d.Id == createResult.Result.Id);

            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authTokenOptions.Key)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(authTokenOptions.Issuer,
                                             authTokenOptions.Audience,
                                             Array.Empty<Claim>(),
                                             expires: DateTime.Now.AddMinutes(-100),
                                             signingCredentials: creds);

            var refreshResult = await mediator.Send(new Refresh.Command
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            });

            refreshResult.Should().NotBeNull();

            refreshResult.Result.Should().BeNull();
            refreshResult.Error.Should().NotBeNull();

            refreshResult.Error.Should().NotBeNull().And.BeOfType<UnauthorizedError>();
        }

        [Fact]
        public async Task Should_Unauthorize_Inexistent_User_Token()
        {
            var createUserCommand = new Faker<Create.Command>()
                                           .RuleFor(d => d.FirstName, d => d.Person.FirstName)
                                           .RuleFor(d => d.LastName, d => d.Person.LastName)
                                           .RuleFor(d => d.Email, d => d.Internet.Email())
                                           .RuleFor(d => d.Password, d => d.Internet.Password())
                                           .Generate();

            var createResult = await mediator.Send(createUserCommand);

            var user = await db.Users.FirstOrDefaultAsync(d => d.Id == createResult.Result.Id);

            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authTokenOptions.Key)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(authTokenOptions.Issuer,
                                             authTokenOptions.Audience,
                                             Array.Empty<Claim>(),
                                             expires: DateTime.Now.AddMinutes(authTokenOptions.TokenDuration),
                                             signingCredentials: creds);

            var refreshResult = await mediator.Send(new Refresh.Command
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            });

            refreshResult.Should().NotBeNull();

            refreshResult.Result.Should().BeNull();
            refreshResult.Error.Should().NotBeNull();

            refreshResult.Error.Should().NotBeNull().And.BeOfType<UnauthorizedError>();
        }
    }
}
