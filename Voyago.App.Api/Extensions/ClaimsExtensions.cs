using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Voyago.App.Api.Extensions;

public static class ClaimsExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        // For some reason asp.net maps the Sub to this weird NameIdentifier claim
        Claim? userIdClaim = context.User.Claims
            .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userIdClaim?.Value, out Guid parsedId))
        {
            return parsedId;
        }

        return null;
    }
}
