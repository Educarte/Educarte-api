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

namespace Api.Features.ContractedHours;

/// <summary>
/// ContractedHours Controller
/// </summary>
[Route("[controller]")]
public class ContractedHoursController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// 
    /// </summary>
    public ContractedHoursController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Delete an contracted hour
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{Id}")]
    [AuthorizeByProfile(Profile.Admin)]
    public Task<Result> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken)
    {
        return mediator.Send(command, cancellationToken);
    }
}
