using Api.Features.Users;
using Api.Results.Users;
using Bogus;
using FluentAssertions;
using Nudes.Retornator.AspnetCore.Errors;
using Xunit;

namespace Test.Features.Users;

public class EditTest : BaseTest
{
    public EditTest(ServerFixture serverFixture) : base(serverFixture)
    {
    }

    [Fact]
    public async Task Should_Edit_User_With_Correct_Data()
    {
        var user = await Setup();

        var command = new Faker<Edit.Command>()
            .RuleFor(d => d.Id, f => user.Id)
            .RuleFor(d => d.FirstName, f => f.Name.FirstName())
            .RuleFor(d => d.LastName, f => f.Name.LastName())
            .Generate();

        var result = await mediator.Send(command);

        result.Should().NotBeNull();

        result.Error.Should().BeNull();

        result.Result.Should().NotBeNull().And.BeOfType<UserResult>();

        result.Result.Id.Should().Be(command.Id).And.Be(user.Id);

        result.Result.FirstName.Should().Be(command.FirstName);

        result.Result.LastName.Should().Be(command.LastName);
    }

    [Fact]
    public async Task Should_Not_Edit_User_With_Empty_Fields()
    {
        var user = await Setup();

        var command = new Faker<Edit.Command>()
            .RuleFor(d => d.Id, f => user.Id)
            .RuleFor(d => d.FirstName, f => String.Empty)
            .RuleFor(d => d.LastName, f => String.Empty)
            .Generate();

        var result = await mediator.Send(command);

        result.Should().NotBeNull();

        result.Result.Should().BeNull();

        result.Error.Should().NotBeNull().And.BeOfType<BadRequestError>();

        result.Error.FieldErrors[nameof(command.FirstName)].Should().NotBeEmpty();

        result.Error.FieldErrors[nameof(command.LastName)].Should().NotBeEmpty();
    }

    private async Task<UserResult> Setup()
    {
        var command = new Faker<Create.Command>()
               .RuleFor(d => d.FirstName, d => d.Person.FirstName)
               .RuleFor(d => d.LastName, d => d.Person.LastName)
               .RuleFor(d => d.Email, d => d.Internet.Email())
               .RuleFor(d => d.Password, d => d.Internet.Password(8))
               .Generate();

        var result = await mediator.Send(command);

        return result.Result;
    }
}
