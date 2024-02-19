using Api.Infrastructure.Security;
using Api.Results.Menus;
using Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nudes.Paginator.Core;
using Nudes.Retornator.Core;

namespace Api.Features.Menus;

/// <summary>
/// Classroom Controller
/// </summary>
[Route("[controller]")]
public class MenusController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// 
    /// </summary>
    public MenusController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Create a menu
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<ResultOf<MenuResult>> Create([FromBody] Create.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit a menu
    /// </summary>
    /// <returns></returns>
    [HttpPut("{id}")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<ResultOf<MenuResult>> Edit([FromBody] Edit.Command command, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        command.Id = id;
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// List all menus
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public Task<ResultOf<PageResult<MenuResult>>> List([FromQuery] List.Query query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Get menu
    /// </summary>
    /// <returns></returns>
    [HttpGet("{Id}")]
    [Authorize]
    public Task<ResultOf<MenuResult>> Get([FromQuery] Detail.Query query, [FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        query.Id = Id;
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Delete a Classroom
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{Id}")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<Result> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Toggle status active Classroom
    /// </summary>
    /// <returns></returns>
    [HttpPatch("{Id}/ToggleActive")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<Result> ToggleStatus([FromRoute] ToggleActive.Command query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }
}
