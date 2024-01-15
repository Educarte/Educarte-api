using Api.Features.Users;
using Api.Results.Users;
using Bogus;
using Core;
using FluentAssertions;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;
using Xunit;

namespace Test.Features.Users;

public class ValidateResetPasswordCodeTest : BaseTest
{

    private readonly ValidateResetPasswordCode.Handler handler;
    public ValidateResetPasswordCodeTest(ServerFixture serverFixture) : base(serverFixture)
    {
        handler = new ValidateResetPasswordCode.Handler(db);
    }

    [Fact]
    public async Task Should_Validate_Password_Code()
    {
        var user = await CreateUser();

        var resetPasswordCode = new ResetPasswordCode
        {
            Code = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.Now.AddMinutes(10),
            UserId = user.Id,
        };

        db.ResetPasswordCodes.Add(resetPasswordCode);

        await db.SaveChangesAsync();

        var result = await handler.Handle(new ValidateResetPasswordCode.Command { Code = resetPasswordCode.Code }, CancellationToken.None);

        result.Should().Be(Result.Success);

    }

    [Fact]
    public async Task Should_Not_Validate_Password_Code_Expired()
    {
        var user = await CreateUser();

        var resetPasswordCode = new ResetPasswordCode
        {
            Code = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.Now.AddMinutes(-1),
            UserId = user.Id,
        };

        db.ResetPasswordCodes.Add(resetPasswordCode);

        await db.SaveChangesAsync();

        var result = await handler.Handle(new ValidateResetPasswordCode.Command { Code = resetPasswordCode.Code }, CancellationToken.None);

        result.Error.Should().BeOfType<BadRequestError>();

    }

    [Fact]
    public async Task Should_Not_Validate_Password_Code_Consumed()
    {
        var user = await CreateUser();

        var resetPasswordCode = new ResetPasswordCode
        {
            Code = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.Now.AddMinutes(10),
            UserId = user.Id,
            ConsumedAt = DateTime.Now
        };

        db.ResetPasswordCodes.Add(resetPasswordCode);

        await db.SaveChangesAsync();

        var result = await handler.Handle(new ValidateResetPasswordCode.Command { Code = resetPasswordCode.Code }, CancellationToken.None);

        result.Error.Should().BeOfType<BadRequestError>();

    }

    [Fact]
    public async Task Should_Not_Validate_Password_Code_Inexistent()
    {
        var result = await handler.Handle(new ValidateResetPasswordCode.Command { Code = Guid.NewGuid().ToString() }, CancellationToken.None);

        result.Error.Should().BeOfType<BadRequestError>();
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
