using Api.Infrastructure.Security;
using Api.Results.AccessControl;
using Api.Results.Generic;
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
    [AuthorizeByProfile(Profile.Admin)]
    public Task<ResultOf<StudentResult>> Create([FromBody] Create.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }
    
    /// <summary>
    /// Add a Legal Guardian
    /// </summary>
    /// <returns></returns>
    [HttpPost("LegalGuardian/{Id}")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<ResultOf<StudentResult>> AddLegalGuardian([FromBody] AddLegalGuardian.Command command, [FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        command.StudentId = Id;
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Add an access control
    /// </summary>
    /// <returns></returns>
    [HttpPost("AccessControl/{Id}")]
    [AuthorizeByProfile(Profile.Admin, Profile.Employee, Profile.Teacher)]
    public Task<ResultOf<MessageResult>> AddAccessControl([FromBody] AddAccessControl.Command command, [FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        command.Id = Id;
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// List mobile
    /// </summary>
    /// <returns></returns>
    [HttpGet("Mobile")]
    [Authorize]
    public Task<ResultOf<MobileListResult<StudentSimpleResult>>> ListMobile([FromQuery] ListMobile.Query query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Add an contracted hour
    /// </summary>
    /// <returns></returns>
    [HttpPost("ContractedHour/{Id}")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<ResultOf<MessageResult>> AddContractedHour([FromBody] AddContractedHour.Command command, [FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        command.Id = Id;
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Edit an student
    /// </summary>
    /// <returns></returns>
    [HttpPut("{id}")]
    [AuthorizeByProfile(Profile.Admin)]
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
    public Task<ResultOf<PageResult<StudentSimpleResult>>> List([FromQuery] List.Query query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Return a student (Mobile)
    /// </summary>
    /// <returns></returns>
    [HttpGet("{Id}/Mobile")]
    [Authorize]
    public Task<ResultOf<StudentResult>> GetMobile([FromQuery] MobileDetail.Query query, [FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        query.Id = Id;
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Return a student
    /// </summary>
    /// <returns></returns>
    [HttpGet("{Id}")]
    [Authorize]
    public Task<ResultOf<StudentResult>> Get([FromQuery] Detail.Query query, [FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        query.Id = Id;
        return mediator.Send(query, cancellationToken);
    }
    
    /// <summary>
    /// Return a student with access controls details
    /// </summary>
    /// <returns></returns>
    [HttpGet("AccessControls/{Id}")]
    [Authorize]
    public Task<ResultOf<AccessControlResult>> GetAccessControlDetails([FromQuery] DetailAccessControl.Query query, [FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        query.Id = Id;
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Delete a student
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{Id}")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<Result> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Toggle status active student
    /// </summary>
    /// <returns></returns>
    [HttpPatch("{Id}/ToggleActive")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<Result> ToggleStatus([FromRoute] ToggleActive.Command query, CancellationToken cancellationToken)
    {
        return mediator.Send(query, cancellationToken);
    }
}
