using Api.Features.Users;
using Api.Infrastructure;
using Api.Infrastructure.Services;
using Core;
using Core.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Test.Features.Users
{
    public class MeTest : BaseTest
    {
        private readonly ServerFixture serverFixture;
        private readonly Me.Handler handler;
        private Mock<IMediator> mediatorMock;
        private Mock<IActor> adminActorMock;
        private User user;

        public MeTest(ServerFixture serverFixture) : base(serverFixture)
        {
            this.serverFixture = serverFixture;
            Setup().GetAwaiter().GetResult();
            handler = new Me.Handler(adminActorMock.Object, mediatorMock.Object);
        }

        [Fact]
        public async Task Should_Return_Correct_User_Detail_With_Actor()
        {
            var result = await handler.Handle(new Me.Query { }, CancellationToken.None);

            result.Should().NotBeNull();
            result.Result.Should().NotBeNull();

            result.Result.Id.Should().Be(user.Id);
            result.Result.Email.Should().Be(user.Email);
            result.Result.FirstName.Should().Be(user.FirstName);
            result.Result.LastName.Should().Be(user.LastName);
        }

        private async Task Setup()
        {
            var hashService = serverFixture.ServiceProvider.GetService<HashService>();

            var (hash, salt) = hashService.Encrypt("senha123");

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

            mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(d => d.Send(It.IsAny<Detail.Query>(), It.IsAny<CancellationToken>()))
                        .Returns(mediator.Send(new Detail.Query
                        {
                            Id = adminActorMock.Object.UserId
                        }));
        }
    }
}
