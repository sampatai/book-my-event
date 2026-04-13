using System.Security.Claims;

namespace Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserGuid(this ClaimsPrincipal? principal)
    {
        string? userGuid = principal?.FindFirstValue("user_id");

        return Guid.TryParse(userGuid, out Guid parsedUserGuid) ?
            parsedUserGuid :
            throw new ApplicationException("User id is unavailable");
    }
    public static long GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue("kid");

        return long.TryParse(userId, out long parsedUserId) ?
            parsedUserId :
            throw new ApplicationException("User id is unavailable");
    }
}
