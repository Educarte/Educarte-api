using Api.Infrastructure.Security;
using Api.Results.Diary;
using Api.Results.Menus;
using Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nudes.Paginator.Core;
using Nudes.Retornator.Core;

namespace Api.Features.Diary;

/// <summary>
/// Diary Controller
/// </summary>
[Route("[controller]")]
public class DiaryController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// 
    /// </summary>
    public DiaryController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Create a diary
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AuthorizeByProfile(Profile.Admin, Profile.Employee)]
    public Task<ResultOf<DiarySimpleResult>> Create([FromBody] Create.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit a diary
    /// </summary>
    /// <returns></returns>
    [HttpPut("{id}")]
    [AuthorizeByProfile(Profile.Admin, Profile.Employee)]
    public Task<ResultOf<DiarySimpleResult>> Edit([FromBody] Edit.Command command, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        command.Id = id;
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// List all diaries
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public Task<ResultOf<PageResult<DiaryResult>>> List([FromQuery] List.Query query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Get diary
    /// </summary>
    /// <returns></returns>
    [HttpGet("{Id}")]
    [Authorize]
    public Task<ResultOf<DiaryResult>> Get([FromQuery] Detail.Query query, [FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        query.Id = Id;
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Delete a diary
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{Id}")]
    [AuthorizeByProfile(Profile.Admin, Profile.Employee)]
    public Task<Result> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Toggle status active diary
    /// </summary>
    /// <returns></returns>
    [HttpPatch("{Id}/ToggleActive")]
    [AuthorizeByProfile(Profile.Admin, Profile.Employee)]
    public Task<Result> ToggleStatus([FromRoute] ToggleActive.Command query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }
}
