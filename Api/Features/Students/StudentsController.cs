using Api.Infrastructure.Security;
using Api.Results.Students;
using Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nudes.Paginator.Core;
using Nudes.Retornator.Core;

namespace Api.Features.Students;

/// <summary>
/// Student Controller
/// </summary>
[Route("[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// 
    /// </summary>
    public StudentsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Create an student
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AuthorizeByProfile(Profile.Admin, Profile.Employee)]
    public Task<ResultOf<StudentResult>> Create([FromBody] Create.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit an student
    /// </summary>
    /// <returns></returns>
    [HttpPut("{id}")]
    [AuthorizeByProfile(Profile.Admin, Profile.Employee)]
    public Task<ResultOf<StudentResult>> Edit([FromBody] Edit.Command command, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        command.Id = id;
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// List all students
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public Task<ResultOf<PageResult<StudentResult>>> List([FromQuery] List.Query query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }

    [HttpGet("{Id}")]
    [Authorize]
    public Task<ResultOf<StudentResult>> Get([FromQuery] Detail.Query query, [FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        query.Id = Id;
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Delete an user
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{Id}")]
    [AuthorizeByProfile(Profile.Admin, Profile.Employee)]
    public Task<Result> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Toggle status active student
    /// </summary>
    /// <returns></returns>
    [HttpPatch("{Id}/ToggleActive")]
    [AuthorizeByProfile(Profile.Admin, Profile.Employee)]
    public Task<Result> ToggleStatus([FromRoute] ToggleActive.Command query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }
}
