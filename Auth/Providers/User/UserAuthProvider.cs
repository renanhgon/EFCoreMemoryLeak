using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Auth.Providers.User;

public class UserAuthProvider(IHttpContextAccessor httpContextAccessor) : IUserAuthProvider
{
    public string Email
        => IsAuthenticated
            ? UserClaims.FindFirstValue(ClaimTypes.Email)!
            : string.Empty;

    public bool IsAuthenticated
            => UserClaims?.Identity?.IsAuthenticated is true;

    public string Role
        => IsAuthenticated
            ? UserClaims.FindFirstValue(ClaimTypes.Role)!
            : string.Empty;

    public Guid UserId
            => IsAuthenticated
            ? Guid.Parse(UserClaims.FindFirstValue(ClaimTypes.NameIdentifier)!)
            : Guid.Empty;

    public string Username
            => IsAuthenticated
            ? UserClaims.FindFirstValue(ClaimTypes.Name)!
            : string.Empty;

    private ClaimsPrincipal UserClaims
        => httpContextAccessor?.HttpContext?.User!;
}