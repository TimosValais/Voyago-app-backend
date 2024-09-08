using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyago.App.Api.Constants;
using Voyago.App.Api.Mappings;
using Voyago.App.BusinessLogic.Services;
using ContractValueObjects = Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Api.Controllers;

[ApiController]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITripTaskService _tripTaskService;

    public TaskController(ITripTaskService tripTaskService)
    {
        _tripTaskService = tripTaskService;
    }

    [HttpPost(ApiRoutes.TaskRoutes.AddUser)]
    public async Task<IActionResult> AddUser([FromRoute] Guid id, [FromRoute] Guid userId, [FromQuery] ContractValueObjects.TaskType type, CancellationToken cancellationToken)
    {

        bool success = await _tripTaskService.AddUser(userId, id, type.MapToEntity(), cancellationToken);
        if (!success)
        {
            return NotFound(new { Message = "Could not add the user to the task." });
        }

        return Ok(new { Message = "User added to the task successfully." });
    }

    [HttpDelete(ApiRoutes.TaskRoutes.RemoveUser)]
    public async Task<IActionResult> RemoveUser([FromRoute] Guid id, [FromRoute] Guid userId, [FromQuery] ContractValueObjects.TaskType type, CancellationToken cancellationToken)
    {
        bool success = await _tripTaskService.RemoveUser(userId, id, type.MapToEntity(), cancellationToken);
        if (!success)
        {
            return NotFound(new { Message = "Could not remove the user from the task." });
        }

        return Ok(new { Message = "User removed from the task successfully." });
    }
}
