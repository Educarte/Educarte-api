using Api.Results.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nudes.Retornator.Core;

namespace Api.Features.Authentication;

/// <summary>
/// Auth Controller
/// </summary>
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// 
    /// </summary>
    public AuthController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Auth user
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public Task<ResultOf<AuthResult>> Auth([FromBody] Auth.Command command)
        => mediator.Send(command);

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost("refresh")]
    public Task<ResultOf<AuthResult>> Refresh()
    {
        Request.Headers.TryGetValue("Authorization", out StringValues bearerToken);

        var command = new Refresh.Command
        {
            Token = bearerToken.FirstOrDefault().Replace("Bearer ", "")
        };

        return mediator.Send(command);
    }
}
