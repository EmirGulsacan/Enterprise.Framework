using System.Reflection;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Security;
using Enterprise.Framework.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Enterprise.Framework.Infrastructure.Persistence;

public class PermissionSeeder
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<PermissionSeeder> _logger;

    public PermissionSeeder(IApplicationDbContext context, ILogger<PermissionSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("System permission seeding started...");

            var existingModules = await _context.GetDbSet<AppModule>()
                .ToDictionaryAsync(m => m.Name);

            var existingModuleSet = existingModules.Keys.ToHashSet();

            var existingPermissionCodes = (await _context.GetDbSet<AppPermission>()
                .AsNoTracking()
                .Select(p => p.Code)
                .ToListAsync())
                .ToHashSet();

            var permissionType = typeof(Permissions);
            var modules = permissionType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            var newModules = new List<AppModule>();
            var newPermissions = new List<AppPermission>();

            foreach (var moduleType in modules)
            {
                var moduleNameField = moduleType.GetField("Module", BindingFlags.Public | BindingFlags.Static);
                var moduleName = moduleNameField?.GetValue(null)?.ToString() ?? moduleType.Name;

                AppModule module;
                if (!existingModuleSet.Contains(moduleName))
                {
                    module = new AppModule { Name = moduleName };
                    newModules.Add(module);
                    existingModuleSet.Add(moduleName);
                    existingModules[moduleName] = module;
                }
                else
                {
                    module = existingModules[moduleName];
                }

                var permissionFields = moduleType
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.Name != "Module");

                foreach (var field in permissionFields)
                {
                    var code = field.GetValue(null)?.ToString();
                    if (string.IsNullOrEmpty(code)) continue;

                    if (!existingPermissionCodes.Contains(code))
                    {
                        newPermissions.Add(new AppPermission
                        {
                            Code = code,
                            Name = field.Name,
                            Module = module
                        });
                        existingPermissionCodes.Add(code);
                    }
                }
            }

            if (newModules.Count > 0)
                _context.GetDbSet<AppModule>().AddRange(newModules);

            if (newPermissions.Count > 0)
                _context.GetDbSet<AppPermission>().AddRange(newPermissions);

            if (newModules.Count > 0 || newPermissions.Count > 0)
                await _context.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("System permission seeding completed. Added {Modules} modules, {Permissions} permissions.",
                newModules.Count, newPermissions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during System permission seeding.");
            throw;
        }
    }
}

