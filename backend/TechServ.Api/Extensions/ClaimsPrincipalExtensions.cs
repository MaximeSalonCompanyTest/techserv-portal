using System.Security.Claims;

namespace TechServ.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    // The user ID always comes from the signed token, never from the URL or the body.
    // Extension method: the "this" keyword lets controllers call it as User.GetUserId().
    // User is the ClaimsPrincipal that UseAuthentication built from the token's claims.
    public static int GetUserId(this ClaimsPrincipal user)
    {
        return int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
