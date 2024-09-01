using Microsoft.AspNetCore.Mvc;
using Voyago.App.Api.Constants;
using Voyago.App.Api.Mappings;
using Voyago.App.BusinessLogic.Services;

namespace Voyago.App.Api.Controllers;
[ApiController]
public class UserProfilesController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;

    public UserProfilesController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }
    [HttpGet(Name = ApiRoutes.UserProfileRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        IEnumerable<DataAccessLayer.Entities.UserProfile> result = await _userProfileService.GetAllAsync(cancellationToken);
        return Ok(result.MapToResponses());
    }
}
