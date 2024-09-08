using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Voyago.App.Contracts.Messages;
using Voyago.App.Contracts.Requests;
using Voyago.Auth.API.Constants;
using Voyago.Auth.API.Mappings;
using Voyago.Auth.BusinessLogic.Exceptions;
using Voyago.Auth.BusinessLogic.Services;

namespace Voyago.Auth.API.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuthController(IAuthService authService, IPublishEndpoint publishEndpoint)
    {
        _authService = authService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost(ApiRoutes.AuthRoutes.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = await _authService.RegisterAsync(request.MapToUser(), cancellationToken);
            await _publishEndpoint.Publish(new UserRegisterMessage(request.Username, request.Email, userId), cancellationToken);
            return Created();
        }
        catch (UserAlreadyExistsException authEx)
        {
            return Conflict(new { Message = authEx.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost(ApiRoutes.AuthRoutes.Login)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            string? token = await _authService.LoginAsync(request.EmailOrUsername, request.Password, cancellationToken);
            if (token == null)
            {
                return Unauthorized(new { Message = "Invalid username, email, or password" });
            }

            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
