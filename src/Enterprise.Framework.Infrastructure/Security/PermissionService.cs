namespace Enterprise.Framework.Infrastructure.Security;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Enterprise.Framework.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

public class PermissionService : IPermissionService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public PermissionService(IServiceScopeFactory scopeFactory, IMemoryCache cache)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
    }

    public async Task<HashSet<string>> GetPermissionsAsync(string identityId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"UserPermissions_{identityId}";

        if (!_cache.TryGetValue(cacheKey, out HashSet<string>? permissions))
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var userRoles = await dbContext.GetDbSet<AppUser>()
                .AsNoTracking()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .Where(u => u.IdentityId == identityId && u.IsActive)
                .SelectMany(u => u.UserRoles)
                .Select(ur => ur.Role)
                .ToListAsync(cancellationToken);

            permissions = userRoles
                .SelectMany(r => r.RolePermissions)
                .Select(rp => rp.Permission.Code)
                .ToHashSet();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(CacheDuration);

            _cache.Set(cacheKey, permissions, cacheOptions);
        }

        return permissions ?? new HashSet<string>();
    }
}
