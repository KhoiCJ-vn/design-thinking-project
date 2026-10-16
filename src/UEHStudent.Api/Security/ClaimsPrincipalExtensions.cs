using System.Security.Claims;

namespace UEHStudent.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var id) ? id : null;
    }

    public static int? GetStudentId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst("StudentId");
        return int.TryParse(claim?.Value, out var id) ? id : null;
    }

    public static string? GetRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Role)?.Value;
    }
}
