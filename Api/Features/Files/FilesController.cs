using Api.Results.Files;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Files;

/// <summary>
/// Files Controller
/// </summary>
[Route("[controller]")]
public class FilesController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// 
    /// </summary>
    public FilesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Upload file
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Results.Files.FileResult>> Upload(IFormCollection form, CancellationToken cancellationToken)
    {
        var toUploadFiles = form.Files.Select(item => new Upload.Command.File
        {
            Name = item.Name,
            Extension = Path.GetExtension(item.FileName),
            FileStream = item.OpenReadStream()
        }).ToList();

        var result = await mediator.Send(new Upload.Command
        {
            Files = toUploadFiles
        }, cancellationToken);

        return Ok(result);
    }
}
