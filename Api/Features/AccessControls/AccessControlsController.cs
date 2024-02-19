using Api.Infrastructure.Security;
using Api.Results.AccessControl;
using Api.Results.Diary;
using Api.Results.Menus;
using Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nudes.Paginator.Core;
using Nudes.Retornator.Core;

namespace Api.Features.AccessControls;

/// <summary>
/// AccessControl Controller
/// </summary>
[Route("[controller]")]
public class AccessControlsController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// 
    /// </summary>
    public AccessControlsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Edit an access control
    /// </summary>
    /// <returns></returns>
    [HttpPut("{id}")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<ResultOf<AccessControlSimpleResult>> Edit([FromBody] Edit.Command command, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        command.Id = id;
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Delete an access control
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{Id}")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<Result> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }
}
