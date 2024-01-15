using Api.Features.Users;
using Bogus;
using Core;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;
using Xunit;

namespace Test.Features.Users;

public class UpdateForgotPasswordTest : BaseTest
{
    private readonly UpdateForgotPassword.Handler handler;
    public UpdateForgotPasswordTest(ServerFixture serverFixture) : base(serverFixture)
    {
        handler = new UpdateForgotPassword.Handler(db, hashService);
    }

    [Fact]
    public async Task Should_Update_Forgot_Password()
    {
        var code = await GenerateCode(10, false);

        var command = new UpdateForgotPassword.Command
        {
            Code = code,
            NewPassword = new Faker().Internet.Password()
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Result.Success);

        var resetPasswordCode = await db.ResetPasswordCodes.Include(d => d.User).FirstOrDefaultAsync(d => d.Code == code);

        resetPasswordCode.Should().NotBeNull();

        resetPasswordCode.ConsumedAt.HasValue.Should().BeTrue();

        hashService.Compare(resetPasswordCode.User.PasswordHash, resetPasswordCode.User.PasswordSalt, command.NewPassword).Should().BeTrue();

    }

    [Fact]
    public async Task Should_Not_Update_Forgot_Password_With_Expired_Code()
    {
        var code = await GenerateCode(-1, false);

        var command = new UpdateForgotPassword.Command
        {
            Code = code,
            NewPassword = new Faker().Internet.Password()
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Error.Should().BeOfType<BadRequestError>();

        var resetPasswordCode = await db.ResetPasswordCodes.Include(d => d.User).FirstOrDefaultAsync(d => d.Code == code);

        resetPasswordCode.Should().NotBeNull();

        resetPasswordCode.ConsumedAt.HasValue.Should().BeFalse();

        hashService.Compare(resetPasswordCode.User.PasswordHash, resetPasswordCode.User.PasswordSalt, command.NewPassword).Should().BeFalse();

    }

    [Fact]
    public async Task Should_Not_Update_Forgot_Password_With_Consumed_Code()
    {
        var code = await GenerateCode(-1, true);

        var command = new UpdateForgotPassword.Command
        {
            Code = code,
            NewPassword = new Faker().Internet.Password()
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Error.Should().BeOfType<BadRequestError>();

        var resetPasswordCode = await db.ResetPasswordCodes.Include(d => d.User).FirstOrDefaultAsync(d => d.Code == code);

        resetPasswordCode.Should().NotBeNull();

        resetPasswordCode.ConsumedAt.HasValue.Should().BeTrue();

        hashService.Compare(resetPasswordCode.User.PasswordHash, resetPasswordCode.User.PasswordSalt, command.NewPassword).Should().BeFalse();

    }

    private async Task<string> GenerateCode(int expirationMinutes, bool consumed)
    {
        var command = new Faker<Create.Command>()
               .RuleFor(d => d.FirstName, d => d.Person.FirstName)
               .RuleFor(d => d.LastName, d => d.Person.LastName)
               .RuleFor(d => d.Email, d => d.Internet.Email())
               .RuleFor(d => d.Password, d => d.Internet.Password(8))
               .Generate();

        var result = await mediator.Send(command);

        var resetPasswordCode = new ResetPasswordCode
        {
            Code = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.Now.AddMinutes(expirationMinutes),
            UserId = result.Result.Id,
            ConsumedAt = consumed ? DateTime.Now : null
        };

        db.ResetPasswordCodes.Add(resetPasswordCode);

        await db.SaveChangesAsync();

        return resetPasswordCode.Code;
    }
}
