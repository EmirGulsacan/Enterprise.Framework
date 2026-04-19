using System.Reflection;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Security;
using Enterprise.Framework.Domain.Entities.Identity;
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

            var existingModuleNames = (await _context.GetDbSet<AppModule>()
                .AsNoTracking()
                .Select(m => m.Name)
                .ToListAsync())
                .ToHashSet();

            var existingPermissionCodes = (await _context.GetDbSet<AppPermission>()
                .AsNoTracking()
                .Select(p => p.Code)
                .ToListAsync())
                .ToHashSet();

            var permissionType = typeof(Permissions);
            var modules = permissionType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            foreach (var moduleType in modules)
            {
                var moduleNameField = moduleType.GetField("Module", BindingFlags.Public | BindingFlags.Static);
                var moduleName = moduleNameField?.GetValue(null)?.ToString() ?? moduleType.Name;

                AppModule module;
                if (!existingModuleNames.Contains(moduleName))
                {
                    module = new AppModule { Name = moduleName };
                    _context.GetDbSet<AppModule>().Add(module);
                    await _context.SaveChangesAsync(CancellationToken.None);
                    existingModuleNames.Add(moduleName);
                }
                else
                {
                    module = await _context.GetDbSet<AppModule>()
                        .FirstAsync(m => m.Name == moduleName);
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
                        _context.GetDbSet<AppPermission>().Add(new AppPermission
                        {
                            Code = code,
                            Name = field.Name,
                            ModuleId = module.Id
                        });
                        existingPermissionCodes.Add(code);
                    }
                }
            }

            await _context.SaveChangesAsync(CancellationToken.None);
            _logger.LogInformation("System permission seeding completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during System permission seeding.");
            throw;
        }
    }
}

