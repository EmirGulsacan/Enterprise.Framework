namespace Enterprise.Framework.Infrastructure.Identity;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class RoleInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var keycloak = scope.ServiceProvider.GetRequiredService<IKeycloakAdminService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<KeycloakAdminService>>();

        try
        {
            logger.LogInformation("Keycloak realm configuration started...");
            await keycloak.EnableEditUsernameAsync();
            await keycloak.ConfigureClientRedirectsAsync("enterprise-ui");
            logger.LogInformation("Keycloak realm configuration completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Keycloak realm configuration failed.");
        }

        try
        {
            logger.LogInformation("Keycloak role synchronization started...");

            var localRoleNames = await context.GetDbSet<AppRole>()
                .AsNoTracking()
                .Select(r => new { r.Name, r.Description })
                .ToListAsync();

            var keycloakRoleNames = await keycloak.GetAllRealmRoleNamesAsync();
            var keycloakRoleSet = keycloakRoleNames.ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missing = localRoleNames
                .Where(r => !keycloakRoleSet.Contains(r.Name))
                .ToList();

            if (!missing.Any())
            {
                logger.LogInformation("All local roles already exist in Keycloak. No sync needed.");
                return;
            }

            logger.LogInformation("{Count} role(s) missing in Keycloak. Creating...", missing.Count);

            foreach (var role in missing)
            {
                try
                {
                    await keycloak.CreateRoleAsync(role.Name, role.Description);
                    logger.LogInformation("Role '{Role}' created in Keycloak.", role.Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to create role '{Role}' in Keycloak.", role.Name);
                }
            }

            logger.LogInformation("Keycloak role synchronization completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Keycloak role synchronization failed.");
        }
    }
}
