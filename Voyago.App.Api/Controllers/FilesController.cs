using Microsoft.AspNetCore.Mvc;
using Voyago.App.Api.Constants;
using Voyago.App.BusinessLogic.Services;

namespace Voyago.App.Api.Controllers;
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }
    [HttpGet(ApiRoutes.FileRoutes.Get)]
    public async Task<IActionResult> GetFile([FromRoute] string fileId, CancellationToken cancellationToken)
    {
        (byte[] File, string ContentType)? bytes = await _fileService.GetFileBytes(fileId, cancellationToken);
        if (bytes is null || bytes?.File is null || string.IsNullOrWhiteSpace(bytes?.ContentType))
        {
            return NotFound();
        }
        return File(bytes?.File!, bytes?.ContentType!);
    }
}
