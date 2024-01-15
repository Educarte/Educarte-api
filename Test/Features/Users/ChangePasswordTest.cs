using Api.Features.Users;
using Api.Infrastructure;
using Api.Results.Users;
using Bogus;
using Core;
using Core.Enums;
using Core.Interfaces;
using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Moq;
using Nudes.Retornator.AspnetCore.Errors;
using Xunit;

namespace Test.Features.Users;

public class ChangePasswordTest : BaseTest
{

    private readonly ChangePassword.Handler handler;
    private Mock<IActor> adminActorMock;
    private User user;

    public ChangePasswordTest(ServerFixture serverFixture) : base(serverFixture)
    {
        Setup().GetAwaiter().GetResult();
        handler = new ChangePassword.Handler(db, adminActorMock.Object, hashService);
    }


    [Fact]
    public async Task Should_Change_User_Password_With_Corect_Data()
    {
        var response = await Setup();

        var command = new Faker<ChangePassword.Command>()
            .RuleFor(d => d.CurrentPassword, f => response.Password)
            .RuleFor(d => d.NewPassword, f => f.Internet.Password(8))
            .Generate();

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();

        result.Error.Should().BeNull();

        result.Result.Should().NotBeNull();

        result.Result.Id.Should().Be(response.User.Id);
        result.Result.FirstName.Should().Be(response.User.FirstName);
        result.Result.LastName.Should().Be(response.User.LastName);

        var user = await db.Users.OnlyActives().FirstOrDefaultAsync(d => d.Id == response.User.Id);

        hashService.Compare(user.PasswordHash, user.PasswordSalt, command.NewPassword).Should().BeTrue();
    }

    [Fact]
    public async Task Should_Not_Change_User_Password_With_Empty_Fields()
    {
        var command = new ChangePassword.Command
        {
            CurrentPassword = String.Empty,
            NewPassword = String.Empty
        };

        var result = await mediator.Send(command);

        result.Should().NotBeNull();

        result.Result.Should().BeNull();

        result.Error.Should().NotBeNull().And.BeOfType<BadRequestError>();

        result.Error.FieldErrors[nameof(command.CurrentPassword)].Should().NotBeEmpty();
        result.Error.FieldErrors[nameof(command.NewPassword)].Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_Not_Change_User_Password_With_Wrong_Password()
    {
        var response = await Setup();

        var command = new Faker<ChangePassword.Command>()
            .RuleFor(d => d.CurrentPassword, f => f.Internet.Password(8))
            .RuleFor(d => d.NewPassword, f => f.Internet.Password(8))
            .Generate();

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();

        result.Should().NotBeNull();

        result.Result.Should().BeNull();

        result.Error.Should().NotBeNull().And.BeOfType<ForbiddenError>();
    }

    private async Task<Response> Setup()
    {
        var password = "senha123";
        var (hash, salt) = hashService.Encrypt(password);

        db.Users.AddRange(new List<User>
            {
                new User
                {
                    Email = "email@email.com",
                    FirstName = "test admin",
                    LastName = "test",
                    Profile = Profile.Admin,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                },
                new User
                {
                   Email = "Other@email.com",
                   FirstName = "test Other",
                   LastName = "test",
                   Profile = Profile.Other,
                   PasswordHash = hash,
                   PasswordSalt = salt,
                },
            });

        await db.SaveChangesAsync();

        user = await db.Users.Where(d => d.Profile == Profile.Admin).FirstOrDefaultAsync();

        adminActorMock = new Mock<IActor>();
        adminActorMock.Setup(d => d.UserId)
                      .Returns(user.Id);

        return new Response
        {
            Password = password,
            User = user.Adapt<UserResult>(),
        };
    }

    internal class Response
    {
        public UserResult User { get; set; }
        public string Password { get; set; }
    }
}
