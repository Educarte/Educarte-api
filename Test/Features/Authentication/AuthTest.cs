using Api.Features.Authentication.Methods;
using Api.Features.Users;
using Bogus;
using FluentAssertions;
using Nudes.Retornator.AspnetCore.Errors;
using Xunit;

namespace Test.Features.Authentication
{
    public class AuthTest : BaseTest
    {
        private ServerFixture serverFixture;

        public AuthTest(ServerFixture serverFixture) : base(serverFixture)
        {
            this.serverFixture = serverFixture;

        }

        [Fact]
        public async Task Should_Authenticate_Existent_User()
        {
            var createUserCommand = new Faker<Create.Command>()
                                            .RuleFor(d => d.FirstName, d => d.Person.FirstName)
                                            .RuleFor(d => d.LastName, d => d.Person.LastName)
                                            .RuleFor(d => d.Email, d => d.Internet.Email())
                                            .RuleFor(d => d.Password, d => d.Internet.Password())
                                            .Generate();

            var createResult = await mediator.Send(createUserCommand);

            var authCommand = new Auth.Command
            {
                Email = createUserCommand.Email,
                Password = createUserCommand.Password
            };

            var loginResult = await mediator.Send(authCommand);

            loginResult.Should().NotBeNull();

            loginResult.Error.Should().BeNull();
            loginResult.Result.Should().NotBeNull();

            loginResult.Result.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Should_Unauthorize_Existent_User_With_Incorrect_Password()
        {
            var createUserCommand = new Faker<Create.Command>()
                                            .RuleFor(d => d.FirstName, d => d.Person.FirstName)
                                            .RuleFor(d => d.LastName, d => d.Person.LastName)
                                            .RuleFor(d => d.Email, d => d.Internet.Email())
                                            .RuleFor(d => d.Password, d => d.Internet.Password())
                                            .Generate();

            var createResult = await mediator.Send(createUserCommand);

            var authCommand = new Auth.Command
            {
                Email = createUserCommand.Email,
                Password = Guid.NewGuid().ToString(),
            };

            var loginResult = await mediator.Send(authCommand);
            loginResult.Should().NotBeNull();

            loginResult.Result.Should().BeNull();
            loginResult.Error.Should().NotBeNull();

            loginResult.Error.Should().NotBeNull().And.BeOfType<UnauthorizedError>();
        }

        [Fact]
        public async Task Should_Unauthorize_Inexistent_User()
        {
            var authCommand = new Faker<Auth.Command>()
                                            .RuleFor(d => d.Email, d => d.Internet.Email())
                                            .RuleFor(d => d.Password, d => d.Internet.Password())
                                            .Generate();

            var loginResult = await mediator.Send(authCommand);

            loginResult.Should().NotBeNull();

            loginResult.Result.Should().BeNull();
            loginResult.Error.Should().NotBeNull();

            loginResult.Error.Should().NotBeNull().And.BeOfType<UnauthorizedError>();
        }
    }
}
