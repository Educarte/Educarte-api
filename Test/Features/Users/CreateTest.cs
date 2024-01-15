using Api.Features.Users;
using Api.Infrastructure.Services;
using Api.Results.Users;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nudes.Retornator.AspnetCore.Errors;
using Xunit;

namespace Test.Features.Users
{
    public sealed class CreateTest : BaseTest
    {
        private readonly HashService hashService;

        public CreateTest(ServerFixture serverFixture) : base(serverFixture)
        {
            hashService = serverFixture.ServiceProvider.GetService<HashService>();
        }


        [Fact]
        public async Task Should_Create_User_On_Database_With_Correct_Data_And_Password()
        {
            var command = new Faker<Create.Command>()
                .RuleFor(d => d.FirstName, d => d.Person.FirstName)
                .RuleFor(d => d.LastName, d => d.Person.LastName)
                .RuleFor(d => d.Email, d => d.Internet.Email())
                .RuleFor(d => d.Password, d => d.Internet.Password(6))
                .Generate();

            var result = await mediator.Send(command);
            result.Should().NotBeNull();

            result.Error.Should().BeNull();

            result.Result.Should().NotBeNull().And.BeOfType<UserResult>();

            var user = await db.Users.FirstOrDefaultAsync(d => d.Id == result.Result.Id);

            user.Should().NotBeNull();

            result.Result.Id.Should().Be(user.Id);
            result.Result.FirstName.Should().Be(command.FirstName).And.Be(user.FirstName);
            result.Result.LastName.Should().Be(command.LastName).And.Be(user.LastName);
            result.Result.Email.Should().Be(command.Email).And.Be(user.Email);

            hashService.Compare(user.PasswordHash, user.PasswordSalt, command.Password).Should().BeTrue();
        }

        [Fact]
        public async Task Should_Not_Create_User_On_Database_With_Empty_Fields()
        {
            var command = new Faker<Create.Command>()
               .RuleFor(d => d.FirstName, d => String.Empty)
               .RuleFor(d => d.LastName, d => String.Empty)
               .RuleFor(d => d.Email, d => String.Empty)
               .RuleFor(d => d.Password, d => String.Empty)
               .Generate();

            var result = await mediator.Send(command);
            result.Should().NotBeNull();

            result.Result.Should().BeNull();

            result.Error.Should().NotBeNull().And.BeOfType<BadRequestError>();

            var fieldErrors = result.Error.FieldErrors.Select(d => d.Key);
            fieldErrors.Should().NotBeEmpty();


            fieldErrors.Should().Contain(nameof(command.FirstName));
            fieldErrors.Should().Contain(nameof(command.LastName));
            fieldErrors.Should().Contain(nameof(command.Email));
            fieldErrors.Should().Contain(nameof(command.Password));
        }

        [Fact]
        public async Task Should_Not_Create_User_With_Invalid_Email_Or_Password()
        {
            var command = new Faker<Create.Command>()
               .RuleFor(d => d.FirstName, d => d.Person.FirstName)
               .RuleFor(d => d.LastName, d => d.Person.LastName)
               .RuleFor(d => d.Email, d => d.Person.FirstName)
               .RuleFor(d => d.Password, d => d.Internet.Password(3))
               .Generate();

            var result = await mediator.Send(command);
            result.Should().NotBeNull();

            result.Result.Should().BeNull();

            result.Error.Should().NotBeNull().And.BeOfType<BadRequestError>();

            var fieldErrors = result.Error.FieldErrors.Select(d => d.Key);
            fieldErrors.Should().NotBeEmpty();

            fieldErrors.Should().Contain(nameof(command.Email));
            fieldErrors.Should().Contain(nameof(command.Password));
        }

        [Fact]
        public async Task Should_Not_Create_User_With_Email_Already_Existent()
        {
            var command = new Faker<Create.Command>()
                .RuleFor(d => d.FirstName, d => d.Person.FirstName)
                .RuleFor(d => d.LastName, d => d.Person.LastName)
                .RuleFor(d => d.Email, d => d.Internet.Email())
                .RuleFor(d => d.Password, d => d.Internet.Password(6))
                .Generate();

            await mediator.Send(command);

            var result = await mediator.Send(command);
            result.Should().NotBeNull();

            result.Result.Should().BeNull();

            result.Error.Should().NotBeNull().And.BeOfType<BadRequestError>();

            var fieldErrors = result.Error.FieldErrors.Select(d => d.Key);
            fieldErrors.Should().NotBeEmpty();

            fieldErrors.Should().Contain(nameof(command.Email));
        }
    }
}
