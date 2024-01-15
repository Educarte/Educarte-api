using Api.Features.Users;
using Api.Infrastructure.Options;
using Api.Infrastructure.Services.Interfaces;
using Api.Results.Users;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Test.Features.Users;

public class RequestResetPasswordTest : BaseTest
{
    private readonly RequestResetPassword.Handler handler;
    private Mock<IEmailService> emailServiceMock;
    private Mock<IOptionsSnapshot<ResetPasswordOptions>> resetPasswordOptionsMock;

    public RequestResetPasswordTest(ServerFixture serverFixture) : base(serverFixture)
    {
        Setup();
        handler = new RequestResetPassword.Handler(db, emailServiceMock.Object, resetPasswordOptionsMock.Object);
    }

    [Fact]
    public async Task Should_Send_Email_With_Password_Code()
    {
        var user = await CreateUser();

        var count = await db.ResetPasswordCodes.Where(d => d.UserId == user.Id).CountAsync();

        await handler.Handle(new RequestResetPassword.Command { Email = user.Email }, CancellationToken.None);

        (await db.ResetPasswordCodes.Where(d => d.UserId == user.Id).ToListAsync()).Should().HaveCount(count + 1);
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

    private void Setup()
    {
        emailServiceMock = new Mock<IEmailService>();

        resetPasswordOptionsMock = new Mock<IOptionsSnapshot<ResetPasswordOptions>>();

        var resetPasswordOptions = new ResetPasswordOptions
        {
            CodeExpirationTime = 10,
            EmailContent = new Faker().Lorem.Paragraph(),
            EmailHtmlContent = new Faker().Lorem.Paragraph(),
            EmailSubject = new Faker().Lorem.Sentence(),
            ResetUri = new Faker().Internet.Url()
        };

        resetPasswordOptionsMock.Setup(d => d.Value)
            .Returns(resetPasswordOptions);
    }
}
