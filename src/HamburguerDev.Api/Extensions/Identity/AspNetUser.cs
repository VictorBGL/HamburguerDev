using HamburguerDev.Business.Interfaces;
using System.Security.Claims;

namespace HamburguerDev.Api.Extensions.Identity;

public class AspNetUser : IUser
{
    private readonly IHttpContextAccessor _contextAccessor;

    public AspNetUser(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public string Name => _contextAccessor.HttpContext?.User.Identity?.Name ?? string.Empty;

    public IEnumerable<Claim> GetClaimsIdentity()
    {
        return _contextAccessor.HttpContext?.User.Claims ?? Enumerable.Empty<Claim>();
    }

    public string GetEmail()
    {
        return IsAuthenticated() ? _contextAccessor.HttpContext!.User.GetEmail() : string.Empty;
    }

    public Guid GetId()
    {
        if (!IsAuthenticated())
        {
            return Guid.Empty;
        }

        var userId = _contextAccessor.HttpContext!.User.GetUserId();
        return Guid.TryParse(userId, out var parsedId) ? parsedId : Guid.Empty;
    }

    public bool IsAuthenticated()
    {
        return _contextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }

    public bool IsInRole(string role)
    {
        return _contextAccessor.HttpContext?.User.IsInRole(role) ?? false;
    }
}

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    public static string GetEmail(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        return principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }
}