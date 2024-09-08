using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyago.App.Api.Constants;
using Voyago.App.Api.Mappings;
using Voyago.App.BusinessLogic.Services;
using Voyago.App.Contracts.Messages;
using Voyago.App.Contracts.Requests;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.Api.Controllers;

[ApiController]
[Route(ApiRoutes.UserProfileRoutes.GetAll)]
[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly ITripTaskService _tripTaskService;
    private readonly ITripService _tripService;
    private readonly ITripUserRolesService _tripUserRolesService;
    private readonly IPublishEndpoint _publishEndpoint;

    public UserProfileController(IUserProfileService userProfileService,
                                ITripTaskService tripTaskService,
                                ITripService tripService,
                                ITripUserRolesService tripUserRolesService,
                                IPublishEndpoint publishEndpoint)
    {
        _userProfileService = userProfileService;
        _tripTaskService = tripTaskService;
        _tripService = tripService;
        _tripUserRolesService = tripUserRolesService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        IEnumerable<UserProfile> userProfiles = await _userProfileService.GetAllAsync(cancellationToken);
        return Ok(userProfiles.MapToResponses());
    }

    [HttpGet(ApiRoutes.UserProfileRoutes.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        UserProfile? userProfile = await _userProfileService.GetByIdAsync(id, cancellationToken);
        if (userProfile == null)
            return NotFound();

        return Ok(userProfile.MapToResponse());
    }

    [HttpPut(ApiRoutes.UserProfileRoutes.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        UserProfile? existingUser = await _userProfileService.GetByIdAsync(id, cancellationToken);
        if (existingUser == null)
            return NotFound();

        UserProfile updatedUserProfile = request.MapToEntity(id);
        bool success = await _userProfileService.UpdateAsync(updatedUserProfile, cancellationToken);
        if (!success)
            return BadRequest(new { Message = "Could not update the user profile." });
        else
        {
            await _publishEndpoint.Publish<UserUpdateMessage>(new(updatedUserProfile.Name, updatedUserProfile.Email, updatedUserProfile.Id));
            if (request.ProfilePicture is not null)
            {
                await _publishEndpoint.Publish<UserProfilePictureUpdateMessage>(new(request.ProfilePicture, id));
            }
        }
        return NoContent();
    }

    [HttpDelete(ApiRoutes.UserProfileRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        bool success = await _userProfileService.DeleteAsync(id, cancellationToken);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpGet(ApiRoutes.UserProfileRoutes.GetTasks)]
    public async Task<IActionResult> GetTasks([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        IEnumerable<TripTask> tasks = await _tripTaskService.GetAllByUserIdAsync(id, cancellationToken);
        return Ok(tasks);
    }

    [HttpGet(ApiRoutes.UserProfileRoutes.GetTrips)]
    public async Task<IActionResult> GetTrips([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        IEnumerable<Trip> trips = await _tripService.GetByUserIdAsync(id, cancellationToken);
        IEnumerable<TripUserRoles> tripUserRoles = await _tripUserRolesService.GetByUserId(id, cancellationToken);
        return Ok(trips.MapToResponses(tripUserRoles));
    }
}
