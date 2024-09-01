namespace Voyago.App.Api.Extensions;

public static class ClaimsExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        System.Security.Claims.Claim? userId = context.User.Claims.SingleOrDefault(c => c.Type == "userId");

        if (Guid.TryParse(userId?.Value, out Guid parsedId))
        {
            return parsedId;
        }

        return null;
    }
}
