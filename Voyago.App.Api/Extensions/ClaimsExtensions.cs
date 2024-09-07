using System.IdentityModel.Tokens.Jwt;

namespace Voyago.App.Api.Extensions;

public static class ClaimsExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        System.Security.Claims.Claim? userIdClaim = context.User.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

        if (Guid.TryParse(userIdClaim?.Value, out Guid parsedId))
        {
            return parsedId;
        }

        return null;
    }
}
