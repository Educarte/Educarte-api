using Api.Features.Users;
using Api.Infrastructure.Services;
using Core;
using Core.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nudes.Retornator.AspnetCore.Errors;
using Xunit;

namespace Test.Features.Users
{
    public class DetailTest : BaseTest
    {
        private readonly ServerFixture serverFixture;
        private readonly Detail.Handler handler;
        private readonly Detail.Validator validator;

        public DetailTest(ServerFixture serverFixture) : base(serverFixture)
        {
            this.serverFixture = serverFixture;
            Setup().GetAwaiter().GetResult();
            handler = new Detail.Handler(db);
            validator = new Detail.Validator();
        }

        [Fact]
        public void Should_Validate_Empty_Id()
        {
            var validationResult = validator.Validate(new Detail.Query
            {
                Id = Guid.Empty
            });
            validationResult.Should().NotBeNull();

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().NotBeEmpty();

            validationResult.Errors.Select(d => d.PropertyName).Should().Contain(nameof(Detail.Query.Id));
        }

        [Fact]
        public void Should_Validate_Valid_Id()
        {
            var validationResult = validator.Validate(new Detail.Query
            {
                Id = Guid.NewGuid()
            });
            validationResult.Should().NotBeNull();

            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_Return_Not_Found()
        {
            var result = await handler.Handle(new Detail.Query
            {
                Id = Guid.NewGuid()
            }, CancellationToken.None);

            result.Should().NotBeNull();
            result.Error.Should().NotBeNull();
            result.Error.Should().BeOfType<NotFoundError>();

            result.Result.Should().BeNull();
        }

        [Fact]
        public async Task Should_Return_User_Details()
        {
            var user = await db.Users.FirstOrDefaultAsync();
            user.Should().NotBeNull();

            var result = await handler.Handle(new Detail.Query
            {
                Id = user.Id
            }, CancellationToken.None);

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
        }
    }
}
