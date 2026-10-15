using System.Security.Claims;

namespace MobileBackEnd.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>UID de Firebase del usuario autenticado (claim "user_id" / "sub" del ID token).</summary>
    public static string? GetUid(this ClaimsPrincipal user) =>
        user.FindFirst("user_id")?.Value
        ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? user.FindFirst("sub")?.Value;

    public static string? GetEmail(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value;
}
