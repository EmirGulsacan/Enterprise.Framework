namespace Enterprise.Framework.API.Middleware;

using Keycloak.Identity.Shared.Interfaces;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class JitProvisioningMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<JitProvisioningMiddleware> _logger;
    private readonly IMemoryCache _cache;

    public JitProvisioningMiddleware(RequestDelegate next, ILogger<JitProvisioningMiddleware> logger, IMemoryCache cache) {
        _next = next;
        _logger = logger;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService, IApplicationDbContext dbContext) {
        if (currentUserService.IsAuthenticated && !string.IsNullOrEmpty(currentUserService.UserId)) {
            var userId = currentUserService.UserId;
            var cacheKey = $"jit_check_{userId}";

            if (!_cache.TryGetValue(cacheKey, out bool exists) || !exists) {
                var userExists = await dbContext.GetDbSet<AppUser>()
                    .AnyAsync(u => u.IdentityId == userId);

                if (!userExists) {
                    try {
                        var newUser = new AppUser {
                            IdentityId = userId,
                            Email = currentUserService.Email ?? string.Empty,
                            FirstName = context.User.FindFirst("given_name")?.Value ?? context.User.Identity?.Name ?? "Admin",
                            LastName = context.User.FindFirst("family_name")?.Value ?? "User",
                            OrganizationId = currentUserService.OrganizationId,
                            BranchId = currentUserService.BranchId,
                            IsActive = true,
                            IsAdmin = currentUserService.IsAdmin 
                        };

                        dbContext.GetDbSet<AppUser>().Add(newUser);
                        await dbContext.SaveChangesAsync(context.RequestAborted);
                        _logger.LogInformation("JIT Provisioning: User {UserId} created successfully.", userId);
                    }
                    catch (DbUpdateException ex) {
                        _logger.LogWarning(ex, "JIT Provisioning: Conflict detected for user {UserId}. Assuming already created by parallel request.", userId);
                    }
                    catch (Exception ex) {
                        _logger.LogError(ex, "JIT Provisioning: Unexpected error for user {UserId}.", userId);
                    }
                }
                _cache.Set(cacheKey, true, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(24) });
            }
        }
        await _next(context);
    }
}
