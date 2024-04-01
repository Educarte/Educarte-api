using Api.Infrastructure.Security;
using Api.Results.Users;
using Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nudes.Paginator.Core;
using Nudes.Retornator.Core;

namespace Api.Features.Users;

/// <summary>
/// Users Controllers
/// </summary>
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// 
    /// </summary>
    public UsersController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// List Users
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<ResultOf<PageResult<UserSimpleResult>>> List([FromQuery] List.Query query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Create an user
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<ResultOf<UserSimpleResult>> Create([FromBody] Create.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit User
    /// </summary>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Authorize]
    public Task<ResultOf<UserSimpleResult>> Edit([FromBody] Edit.Command command, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        command.Id = id;
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Toggle User status
    /// </summary>
    /// <returns></returns>
    [HttpPatch("{Id}/ToggleActive")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<Result> ToggleActive([FromRoute] ToggleActive.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Detail User
    /// </summary>
    /// <returns></returns>
    [HttpGet("{Id}")]
    [Authorize]
    public Task<ResultOf<UserSimpleResult>> Detail([FromRoute] Detail.Query query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Detail logged user
    /// </summary>
    /// <returns></returns>
    [HttpGet("Me")]
    [Authorize]
    public Task<ResultOf<UserSimpleResult>> Me([FromQuery] Me.Query query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Get user by name or email
    /// </summary>
    /// <returns></returns>
    [HttpGet("Get")]
    [Authorize]
    public Task<ResultOf<UserSimpleResult>> Get([FromQuery] Get.Query command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }
    
    /// <summary>
    /// Change User Password
    /// </summary>
    /// <returns></returns>
    [HttpPut("ChangePassword")]
    [Authorize]
    public Task<ResultOf<UserSimpleResult>> ChangePassword([FromBody] ChangePassword.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Send 4 digits Code to Reset Password
    /// </summary>
    /// <returns></returns>
    [HttpPost("MobileRequestResetPassword")]
    public Task<Result> MobileResetPassword([FromBody] MobileRequestResetPassword.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }
    /// <summary>
    /// Send Email to Reset Password
    /// </summary>
    /// <returns></returns>
    [HttpPost("RequestResetPassword")]
    public Task<Result> RequestResetPassword([FromBody] RequestResetPassword.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Validate reset code to change forgotten password
    /// </summary>
    /// <returns></returns>
    [HttpGet("ValidateResetPasswordCode")]
    public Task<Result> ValidateResetPasswordCode([FromQuery] ValidateResetPasswordCode.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Update password after reset code validation
    /// </summary>
    /// <returns></returns>
    [HttpPost("UpdateForgotPassword")]
    public Task<Result> UpdateForgotPassword([FromBody] UpdateForgotPassword.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Delete an User
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{Id}")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<Result> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Reset user password
    /// </summary>
    /// <returns></returns>
    [HttpPatch("{Id}/ResetPassword")]
    [Authorize]
    public Task<Result> ResetPassword(ResetPassword.Command command, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        command.UserId = id;
        return mediator.Send(command, cancellationToken);
    }
}
