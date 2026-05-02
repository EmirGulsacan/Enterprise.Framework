namespace Enterprise.Framework.Infrastructure.Security;

using Enterprise.Framework.Application.Common.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Enterprise.Framework.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

public class PermissionService : IPermissionService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public PermissionService(IServiceScopeFactory scopeFactory, IDistributedCache cache)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
    }

    public async Task<HashSet<string>> GetPermissionsAsync(string identityId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"UserPermissions_{identityId}";

        var cachedBytes = await _cache.GetAsync(cacheKey, cancellationToken);
        if (cachedBytes != null)
        {
            var cachedPermissions = JsonSerializer.Deserialize<HashSet<string>>(cachedBytes);
            if (cachedPermissions != null)
            {
                return cachedPermissions;
            }
        }

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var rolePerms = await dbContext.GetDbSet<AppUser>()
            .AsNoTracking()
            .Where(u => u.IdentityId == identityId && u.IsActive)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync(cancellationToken);

        var overrides = await dbContext.GetDbSet<AppUserPermission>()
            .AsNoTracking()
            .Where(up => up.User.IdentityId == identityId)
            .Select(up => new { up.Permission.Code, up.IsGranted })
            .ToListAsync(cancellationToken);

        var granted = overrides.Where(o => o.IsGranted).Select(o => o.Code);
        var revoked = overrides.Where(o => !o.IsGranted).Select(o => o.Code).ToHashSet();

        var permissions = rolePerms
            .Concat(granted)
            .Where(p => !revoked.Contains(p))
            .Distinct()
            .ToHashSet();

        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheDuration };
        var bytesToCache = JsonSerializer.SerializeToUtf8Bytes(permissions);
        await _cache.SetAsync(cacheKey, bytesToCache, options, cancellationToken);

        return permissions;
    }
}
