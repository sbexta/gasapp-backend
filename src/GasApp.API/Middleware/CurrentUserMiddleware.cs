using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Enums;
using System.Security.Claims;

namespace GasApp.API.Middleware;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid UserId =>
        Guid.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier) ??
                       User?.FindFirstValue("sub"), out var id) ? id : Guid.Empty;

    public string Email => User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    public UserRole Role =>
        Enum.TryParse<UserRole>(User?.FindFirstValue(ClaimTypes.Role), out var role) ? role : UserRole.ClientUser;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
