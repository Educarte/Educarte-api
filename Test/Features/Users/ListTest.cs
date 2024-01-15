using Api.Features.Users;
using Api.Results.Users;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nudes.Paginator.Core;
using Xunit;

namespace Test.Features.Users;

public class ListTest : BaseTest
{
    public ListTest(ServerFixture serverFixture) : base(serverFixture)
    {
    }

    [Fact]
    public async Task Should_Return_List_of_Users()
    {
        var query = new List.Query();

        var result = await mediator.Send(query);
        result.Should().NotBeNull();

        result.Error.Should().BeNull();
        result.Result.Should().NotBeNull().And.BeOfType<PageResult<UserResult>>();
    }

    [Fact]
    public async Task Should_Return_Filtered_List_Of_Users()
    {
        var user = await CreateUser();

        var query = new List.Query() { Search = user.FirstName.Substring(0, 2) };

        var result = await mediator.Send(query);

        result.Error.Should().BeNull();

        result.Result.Should().NotBeNull();

        if (result.Result.Items.Any())
        {
            result.Result.Items.Should().AllSatisfy(item =>
            {
                item.Should().Match<UserResult>(d => d.FirstName.Contains(query.Search)
                                                  || d.LastName.Contains(query.Search)
                                                  || d.Email.Contains(query.Search));
            });
        }
    }

    [Fact]
    public async Task Should_Return_List_Of_Users_Without_Deleted_Users()
    {
        await CreateUser();

        var response = await CreateUser();

        var command = new Delete.Command { Id = response.Id };

        await mediator.Send(command);

        var result = await mediator.Send(new List.Query());

        result.Should().NotBeNull();

        result.Result.Should().NotBeNull().And.BeOfType<PageResult<UserResult>>();

        result.Error.Should().BeNull();

        if (result.Result.Items.Any())
        {
            var dbUsers = await db.Users.Where(d => result.Result.Items.Select(d => d.Id).Contains(d.Id)).ToListAsync();

            dbUsers.Should().HaveCount(result.Result.Items.Count);

            dbUsers.Should().AllSatisfy(d =>
            {
                d.DeletedAt.HasValue.Should().BeFalse();
            });

            result.Result.Items.Select(d => d.Id).Should().NotContain(response.Id);
            dbUsers.Select(d => d.Id).Should().NotContain(response.Id);
        }
    }

    private async Task<UserResult> CreateUser()
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
