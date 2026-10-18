using System.Security.Claims;

namespace TechServ.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    // The user ID always comes from the signed token, never from the URL or the body.
    public static int GetUserId(this ClaimsPrincipal user)
    {
        return int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
