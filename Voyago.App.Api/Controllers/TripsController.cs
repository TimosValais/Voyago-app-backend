using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyago.App.Api.Constants;
using Voyago.App.Api.Extensions;
using Voyago.App.Api.Helpers;
using Voyago.App.Api.Mappings;
using Voyago.App.BusinessLogic.Services;
using Voyago.App.Contracts.Messages;
using Voyago.App.Contracts.Requests;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.ValueObjects;
using ContractValueObjects = Voyago.App.Contracts.ValueObjects;
using EntityValueObjects = Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.Api.Controllers;

[ApiController]
[Route(ApiRoutes.TripRoutes.GetAll)]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;
    private readonly ITripTaskService _tripTaskService;
    private readonly IPublishEndpoint _publishEndpoint;

    public TripsController(ITripService tripService,
                           ITripTaskService tripTaskService,
                           IPublishEndpoint publishEndpoint)
    {
        _tripService = tripService;
        _tripTaskService = tripTaskService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId == null) return Unauthorized();

        IEnumerable<Trip> trips = await _tripService.GetByUserIdAsync(userId.Value, cancellationToken);
        return Ok(trips);
    }

    [HttpGet(ApiRoutes.TripRoutes.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        Trip? trip = await _tripService.GetByIdAsync(id, cancellationToken);
        if (trip == null) return NotFound();

        EntityValueObjects.TripRole? role = GetUserRole(trip);
        if (role == null) return Unauthorized();
        return Ok(trip.MapToResponse((EntityValueObjects.TripRole)role));
    }

    [HttpPost(ApiRoutes.TripRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTripRequest trip, CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId == null) return Unauthorized();
        Guid tripId = Guid.NewGuid();
        bool success = await _tripService.UpsertAsync(trip.MapToEntity(tripId), cancellationToken);
        if (!success) return BadRequest(new { Message = "Could not create the trip." });
        else
        {
            await _tripService.UpsertUser((Guid)userId, tripId, TripRole.Admin, cancellationToken);
        }
        return Created();
    }

    [HttpPut(ApiRoutes.TripRoutes.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTripRequest trip, CancellationToken cancellationToken)
    {
        if (!await HasManagerRights(id, cancellationToken)) return Unauthorized();
        bool success = await _tripService.UpsertAsync(trip.MapToEntity(id), cancellationToken);
        if (!success) return BadRequest(new { Message = "Could not update the trip." });

        return NoContent();
    }

    [HttpDelete(ApiRoutes.TripRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (!await HasAdminRights(id, cancellationToken)) return Unauthorized();
        bool success = await _tripService.DeleteAsync(id, cancellationToken);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpGet(ApiRoutes.TripRoutes.GetTasks)]
    public async Task<IActionResult> GetTasks([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        IEnumerable<TripTask> tasks = await _tripTaskService.GetAllByTripIdAsync(id, cancellationToken);
        return Ok(tasks);
    }

    [HttpPost(ApiRoutes.TripRoutes.PostTask)]
    public async Task<IActionResult> AddTask([FromRoute] Guid id, [FromBody] ITaskRequest task, CancellationToken cancellationToken)
    {
        if (!await HasManagerRights(id, cancellationToken)) return Unauthorized();
        Guid? userId = HttpContext.GetUserId();
        if (userId == null) return Unauthorized();
        Guid taskId = Guid.NewGuid();
        bool success = await _tripTaskService.UpsertAsync(task.MapCreateRequestToEntity(id, taskId), cancellationToken);
        if (!success) return BadRequest(new { Message = "Could not add the task to the trip." });
        else
        {
            await _tripTaskService.AddUser((Guid)userId, taskId, task.TaskType.MapToEntity());
            IEnumerable<byte[]>? documents = TaskHelpers.GetCreateTaskDocuments(task);
            if (documents is not null && documents.Any())
            {
                foreach (byte[] document in documents)
                {
                    await _publishEndpoint.Publish(new TaskFileUpdateMessage(id, document, task.TaskType));
                }
            }
        }
        return CreatedAtAction(nameof(GetTasks), new { id }, task);
    }

    [HttpPut(ApiRoutes.TripRoutes.UpdateTask)]
    public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] ITaskRequest task, CancellationToken cancellationToken)
    {
        if (!await HasManagerRights(id, cancellationToken)) return Unauthorized();

        bool success = await _tripTaskService.UpsertAsync(task.MapUpdateRequestToEntity(id), cancellationToken);
        if (!success) return BadRequest(new { Message = "Could not update the task." });
        else
        {
            IEnumerable<byte[]>? documents = TaskHelpers.GetUpdateTaskDocuments(task);
            if (documents is not null && documents.Any())
            {
                foreach (byte[] document in documents)
                {
                    await _publishEndpoint.Publish(new TaskFileUpdateMessage(id, document, task.TaskType));
                }
            }
        }
        return NoContent();
    }

    [HttpDelete(ApiRoutes.TripRoutes.DeleteTask)]
    public async Task<IActionResult> DeleteTask([FromRoute] Guid id, [FromRoute] Guid taskId, [FromQuery] ContractValueObjects.TaskType type, CancellationToken cancellationToken)
    {
        if (!await HasAdminRights(id, cancellationToken)) return Unauthorized();
        bool success = await _tripTaskService.DeleteAsync(taskId, type.MapToEntity(), cancellationToken);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpPost(ApiRoutes.TripRoutes.PostUser)]
    public async Task<IActionResult> AddUser([FromRoute] Guid id, [FromRoute] Guid userId, [FromQuery] ContractValueObjects.TripRole role, CancellationToken cancellationToken)
    {
        if (!await HasManagerRights(id, cancellationToken)) return Unauthorized();
        bool success = await _tripService.UpsertUser(userId, id, role.MapToEntityTripRole(), cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPut(ApiRoutes.TripRoutes.UpdateUser)]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromRoute] Guid userId, [FromQuery] ContractValueObjects.TripRole role, CancellationToken cancellationToken)
    {
        if (!await HasManagerRights(id, cancellationToken)) return Unauthorized();
        bool success = await _tripService.UpsertUser(userId, id, role.MapToEntityTripRole(), cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }
    [HttpDelete(ApiRoutes.TripRoutes.DeleteUser)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id, [FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        if (!await HasAdminRights(id, cancellationToken)) return Unauthorized();
        bool success = await _tripService.RemoveUser(userId, id, cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }
    private EntityValueObjects.TripRole? GetUserRole(Trip trip)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId == null) return null;

        TripUserRoles? userRole = trip.TripUsers.FirstOrDefault(tur => tur.UserId == userId);

        return userRole?.Role;
    }
    private async Task<bool> HasAdminRights(Guid tripId, CancellationToken cancellationToken = default)
    {
        Trip? trip = await _tripService.GetByIdAsync(tripId, cancellationToken);
        if (trip == null) return false;
        EntityValueObjects.TripRole? role = GetUserRole(trip);
        if (role != EntityValueObjects.TripRole.Admin) return false;
        return true;
    }
    private async Task<bool> HasManagerRights(Guid tripId, CancellationToken cancellationToken = default)
    {
        Trip? trip = await _tripService.GetByIdAsync(tripId, cancellationToken);
        if (trip == null) return false;
        EntityValueObjects.TripRole? role = GetUserRole(trip);
        if (role < EntityValueObjects.TripRole.Manager) return false;
        return true;
    }
}
