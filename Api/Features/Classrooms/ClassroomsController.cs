using Api.Infrastructure.Security;
using Api.Results.Classroom;
using Api.Results.Students;
using Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nudes.Paginator.Core;
using Nudes.Retornator.Core;

namespace Api.Features.Classrooms;

/// <summary>
/// Classroom Controller
/// </summary>
[Route("[controller]")]
public class ClassroomController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// 
    /// </summary>
    public ClassroomController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Create a Classroom
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<ResultOf<ClassroomResult>> Create([FromBody] Create.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit a Classroom
    /// </summary>
    /// <returns></returns>
    [HttpPut("{id}")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<ResultOf<ClassroomBasicResult>> Edit([FromBody] Edit.Command command, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        command.Id = id;
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// List all Classrooms
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public Task<ResultOf<PageResult<ClassroomStudentsSimpleResult>>> List([FromQuery] List.Query query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }

    [HttpGet("{Id}")]
    [Authorize]
    public Task<ResultOf<ClassroomResult>> Get([FromQuery] Detail.Query query, [FromRoute] Guid Id, CancellationToken cancellationToken)
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
