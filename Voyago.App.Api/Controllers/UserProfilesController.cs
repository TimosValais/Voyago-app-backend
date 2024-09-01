using Microsoft.AspNetCore.Mvc;
using Voyago.App.Api.Constants;

namespace Voyago.App.Api.Controllers;
[ApiController]
public class UserProfilesController : ControllerBase
{
    public UserProfilesController()
    {

    }
    [HttpGet(Name = ApiRoutes.UserProfileRoutes.GetAll)]
    public async Task<IEnumerable<>> GetAll(CancellationToken cancellationToken)
    {

    }
}
