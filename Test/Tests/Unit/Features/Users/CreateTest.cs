using Api.Features.Users;
using Api.Infrastructure.Services;
using Api.Infrastructure.Validators;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nudes.Retornator.AspnetCore.Errors;
using Xunit;

namespace Test.Tests.Unit.Features.Users
{
    public sealed class CreateTest : BaseTest
    {
        private readonly HashService hashService;
        private readonly Create.Handler handler;
        private Create.Validator validator;

        public CreateTest(ServerFixture serverFixture) : base(serverFixture)
        {
            hashService = serverFixture.ServiceProvider.GetService<HashService>();
            handler = new Create.Handler(db, hashService);
            validator = new Create.Validator(new UniqueUserValidator<Create.Command>(db));
        }


        [Fact]
        public async Task Should_Create_User_On_Database_With_Correct_Data_And_Password()
        {
            var command = new Faker<Create.Command>()
                .RuleFor(d => d.Name, d => d.Person.FirstName)
                .RuleFor(d => d.SupplierName, d => d.Person.LastName)
                .RuleFor(d => d.Email, d => d.Internet.Email())
                .RuleFor(d => d.Password, d => d.Internet.Password(6))
                .Generate();

            var result = await handler.Handle(command, default);
            result.Should().NotBeNull();

            result.Error.Should().BeNull();

            result.Result.Should().NotBeNull().And.BeOfType<ProductResult>();

            var user = await db.Users.FirstOrDefaultAsync(d => d.Id == result.Result.Id);

            user.Should().NotBeNull();

            result.Result.Id.Should().Be(user.Id);
            result.Result.FirstName.Should().Be(command.Name).And.Be(user.Name);
            result.Result.LastName.Should().Be(command.SupplierName).And.Be(user.SupplierName);
            result.Result.Email.Should().Be(command.Email).And.Be(user.Email);

            hashService.Compare(user.PasswordHash, user.PasswordSalt, command.Password).Should().BeTrue();
        }

        [Fact]
        public async Task Should_Not_Create_User_On_Database_With_Empty_Fields()
        {
            var command = new Faker<Create.Command>()
               .RuleFor(d => d.Name, d => string.Empty)
               .RuleFor(d => d.SupplierName, d => string.Empty)
               .RuleFor(d => d.Email, d => string.Empty)
               .RuleFor(d => d.Password, d => string.Empty)
               .Generate();

            var validationResult = await validator.ValidateAsync(command, default);
            validationResult.IsValid.Should().BeFalse();

            var result = await handler.Handle(command, default);
            result.Should().NotBeNull();

            result.Result.Should().BeNull();

            result.Error.Should().NotBeNull().And.BeOfType<BadRequestError>();

            var fieldErrors = result.Error.FieldErrors.Select(d => d.Key);
            fieldErrors.Should().NotBeEmpty();


            fieldErrors.Should().Contain(nameof(command.Name));
            fieldErrors.Should().Contain(nameof(command.SupplierName));
            fieldErrors.Should().Contain(nameof(command.Email));
            fieldErrors.Should().Contain(nameof(command.Password));
        }

        [Fact]
        public async Task Should_Not_Create_User_With_Invalid_Email_Or_Password()
        {
            var command = new Faker<Create.Command>()
               .RuleFor(d => d.Name, d => d.Person.FirstName)
               .RuleFor(d => d.SupplierName, d => d.Person.LastName)
               .RuleFor(d => d.Email, d => d.Person.FirstName)
               .RuleFor(d => d.Password, d => d.Internet.Password(3))
               .Generate();

            var result = await handler.Handle(command, default);
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
                .RuleFor(d => d.Name, d => d.Person.FirstName)
                .RuleFor(d => d.SupplierName, d => d.Person.LastName)
                .RuleFor(d => d.Email, d => d.Internet.Email())
                .RuleFor(d => d.Password, d => d.Internet.Password(6))
                .Generate();

            await handler.Handle(command, default);

            var result = await handler.Handle(command, default);
            result.Should().NotBeNull();

            result.Result.Should().BeNull();

            result.Error.Should().NotBeNull().And.BeOfType<BadRequestError>();

            var fieldErrors = result.Error.FieldErrors.Select(d => d.Key);
            fieldErrors.Should().NotBeEmpty();

            fieldErrors.Should().Contain(nameof(command.Email));
        }
    }
}
