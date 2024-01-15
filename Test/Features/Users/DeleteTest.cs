using Api.Features.Users;
using Api.Results.Users;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;
using Xunit;

namespace Test.Features.Users;

public class DeleteTest : BaseTest
{
    public DeleteTest(ServerFixture serverFixture) : base(serverFixture)
    {
    }

    [Fact]
    public async Task Should_Delete_User_With_Correct_Data()
    {
        var user = await Setup();

        var command = new Delete.Command { Id = user.Id };

        var result = await mediator.Send(command);

        result.Should().NotBeNull().And.Be(Result.Success);

        result.Error.Should().BeNull();

        var deletedUser = await db.Users.FirstOrDefaultAsync(d => d.Id == command.Id);

        deletedUser.Should().NotBeNull();
        deletedUser.DeletedAt.HasValue.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Not_Delete_User_Already_Deleted()
    {

        var user = await Setup();

        var command = new Delete.Command { Id = user.Id };

        await mediator.Send(command);

        var result = await mediator.Send(command);

        result.Should().NotBeNull();

        result.Error.Should().NotBeNull().And.BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task Should_Not_Delete_User_With_Inexistent_Id()
    {
        var command = new Delete.Command { Id = Guid.NewGuid() };

        await mediator.Send(command);

        var result = await mediator.Send(command);

        result.Should().NotBeNull();

        result.Error.Should().NotBeNull().And.BeOfType<NotFoundError>();
    }


    private async Task<UserResult> Setup()
    {
        var command = new Faker<Create.Command>()
                .RuleFor(d => d.FirstName, d => d.Person.FirstName)
                .RuleFor(d => d.LastName, d => d.Person.LastName)
                .RuleFor(d => d.Email, d => d.Internet.Email())
                .RuleFor(d => d.Password, d => d.Internet.Password(6))
                .Generate();

        var result = await mediator.Send(command);

        return result.Result;
    }
}
