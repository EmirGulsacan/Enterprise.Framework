namespace Enterprise.Framework.Infrastructure;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Infrastructure.Identity;
using Enterprise.Framework.Infrastructure.Persistence;
using Enterprise.Framework.Infrastructure.Persistence.Interceptors;
using Enterprise.Framework.Infrastructure.Persistence.Providers;
using Enterprise.Framework.Infrastructure.Security;
using Enterprise.Framework.Infrastructure.Services;
using Enterprise.Framework.Infrastructure.Services.Communication;
using Enterprise.Framework.Infrastructure.Services.Files;
using Keycloak.Identity.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddSingleton<Enterprise.Framework.Domain.Common.IDateTimeProvider, SystemDateTimeProvider>();

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<DispatchDomainEventsInterceptor>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IKeycloakAdminService, KeycloakAdminService>();

        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<ISmsService, MockSmsService>();

        services.AddScoped<IFileService, LocalFileService>();
        services.AddScoped<IExcelService, ExcelService>();

        var databaseOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();

        if (databaseOptions.Provider?.ToLower() == "postgresql")
        {
            services.AddSingleton<IDatabaseProvider, PostgreSqlDatabaseProvider>();
        }
        else
        {
            services.AddSingleton<IDatabaseProvider, SqlServerDatabaseProvider>();
        }

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var databaseProvider = sp.GetRequiredService<IDatabaseProvider>();
            databaseProvider.Configure(options, databaseOptions.ConnectionString);
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }
}
