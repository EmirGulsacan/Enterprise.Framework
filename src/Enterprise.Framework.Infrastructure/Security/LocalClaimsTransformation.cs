using Enterprise.Framework.Domain.Entities.Identity;
using Enterprise.Framework.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Enterprise.Framework.Infrastructure.Security;

public class LocalClaimsTransformation : IClaimsTransformation
{
    private const string PermsCachePrefix = "framework:perms:v1:";
    private const string UserIdCachePrefix = "framework:userid:v1:";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;

    public LocalClaimsTransformation(IServiceScopeFactory scopeFactory, IMemoryCache cache)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not { IsAuthenticated: true })
            return principal;

        var identityId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub");

        if (string.IsNullOrEmpty(identityId))
            return principal;

        var isAdmin = IsKeycloakAdmin(principal);

        var cacheKey = $"{PermsCachePrefix}{identityId}";

        (List<string> permissions, long localUserId) = await _cache.GetOrCreateAsync(
            cacheKey,
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);

                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<Enterprise.Framework.Application.Common.Interfaces.IApplicationDbContext>();

                List<string> perms;
                if (isAdmin)
                {
                    perms = await context.GetDbSet<AppPermission>()
                        .AsNoTracking()
                        .Select(p => p.Code)
                        .ToListAsync();
                }
                else
                {
                    var rolePerms = await context.GetDbSet<AppUser>()
                        .AsNoTracking()
                        .Where(u => u.IdentityId == identityId)
                        .SelectMany(u => u.UserRoles)
                        .SelectMany(ur => ur.Role.RolePermissions)
                        .Select(rp => rp.Permission.Code)
                        .Distinct()
                        .ToListAsync();

                    var overrides = await context.GetDbSet<AppUserPermission>()
                        .AsNoTracking()
                        .Where(up => up.User.IdentityId == identityId)
                        .Select(up => new { up.Permission.Code, up.IsGranted })
                        .ToListAsync();

                    var granted = overrides.Where(o => o.IsGranted).Select(o => o.Code);
                    var revoked = overrides.Where(o => !o.IsGranted).Select(o => o.Code).ToHashSet();

                    perms = rolePerms
                        .Concat(granted)
                        .Where(p => !revoked.Contains(p))
                        .Distinct()
                        .ToList();
                }

                var userId = await context.GetDbSet<AppUser>()
                    .AsNoTracking()
                    .Where(u => u.IdentityId == identityId)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();

                return (perms, userId);
            })!;

        if (permissions.Count == 0 && !isAdmin)
            return principal;

        var clone = principal.Clone();
        var identity = (ClaimsIdentity)clone.Identity!;

        foreach (var perm in permissions)
        {
            if (!identity.HasClaim("Permission", perm))
                identity.AddClaim(new Claim("Permission", perm));
        }

        if (isAdmin && !identity.HasClaim("IsAdmin", "true"))
            identity.AddClaim(new Claim("IsAdmin", "true"));

        if (localUserId > 0 && !identity.HasClaim("LocalUserId", localUserId.ToString()))
            identity.AddClaim(new Claim("LocalUserId", localUserId.ToString()));

        return clone;
    }

    private static bool IsKeycloakAdmin(ClaimsPrincipal principal)
    {
        var realmAccessClaim = principal.FindFirstValue("realm_access");
        if (!string.IsNullOrEmpty(realmAccessClaim))
        {
            try
            {
                var json = System.Text.Json.JsonDocument.Parse(realmAccessClaim);
                if (json.RootElement.TryGetProperty("roles", out var roles))
                {
                    foreach (var role in roles.EnumerateArray())
                    {
                        var roleName = role.GetString();
                        if (roleName == "admin" || roleName == "framework-admin")
                            return true;
                    }
                }
            }
            catch { }
        }

        return principal.IsInRole("admin");
    }
}

