namespace Enterprise.Framework.Infrastructure.Security;

using Enterprise.Framework.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Enterprise.Framework.Application.Common.Interfaces.IPermissionService _permissionService;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, Enterprise.Framework.Application.Common.Interfaces.IPermissionService permissionService)
    {
        _httpContextAccessor = httpContextAccessor;
        _permissionService = permissionService;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("sub");

    public string? Email => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public string? OrganizationId => _httpContextAccessor.HttpContext?.User.FindFirstValue("OrganizationId");

    public string? BranchId => _httpContextAccessor.HttpContext?.User.FindFirstValue("BranchId") ??
        _httpContextAccessor.HttpContext?.User.FindFirstValue("branch_id");

    public long? LocalUserId
    {
        get
        {
            var val = _httpContextAccessor.HttpContext?.User.FindFirstValue("LocalUserId");
            return long.TryParse(val, out var id) ? id : null;
        }
    }

    public bool IsAdmin =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue("IsAdmin") == "true" ||
        (_httpContextAccessor.HttpContext?.User.IsInRole("admin") ?? false);

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public IReadOnlyList<string> Permissions =>
        _httpContextAccessor.HttpContext?.User.FindAll("Permission").Select(c => c.Value).ToList() ?? new List<string>();

    public async Task<bool> HasPermissionAsync(string permission, CancellationToken cancellationToken = default)
    {
        if (Permissions.Contains(permission))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(UserId))
        {
            return false;
        }

        var permissions = await _permissionService.GetPermissionsAsync(UserId, cancellationToken);
        return permissions.Contains(permission);
    }
}
