namespace Enterprise.Framework.Infrastructure;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Caching;
using Enterprise.Framework.Application.Common.Idempotency;
using Enterprise.Framework.Domain.Common;
using Enterprise.Framework.Infrastructure.Caching;
using Enterprise.Framework.Infrastructure.Idempotency;
using Enterprise.Framework.Infrastructure.Identity;
using Enterprise.Framework.Infrastructure.Persistence;
using Enterprise.Framework.Infrastructure.Persistence.Interceptors;
using Enterprise.Framework.Infrastructure.Persistence.Providers;
using Enterprise.Framework.Infrastructure.Security;
using Enterprise.Framework.Infrastructure.Services;
using Enterprise.Framework.Infrastructure.Services.Communication;
using Enterprise.Framework.Infrastructure.Services.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDistributedMemoryCache();
        services.AddSingleton<IAppCache, DistributedAppCache>();
        services.AddSingleton<IIdempotencyStore, DistributedIdempotencyStore>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<DispatchDomainEventsInterceptor>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddHttpClient<IKeycloakAdminService, KeycloakAdminService>();
        services.AddTransient<Microsoft.AspNetCore.Authentication.IClaimsTransformation, LocalClaimsTransformation>();

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

            options.AddInterceptors(
                sp.GetRequiredService<SoftDeleteInterceptor>(),
                sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>(),
                sp.GetRequiredService<DispatchDomainEventsInterceptor>()
            );
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<PermissionSeeder>();

        services.AddHostedService<Enterprise.Framework.Infrastructure.BackgroundJobs.OutboxProcessorBackgroundService>();

        return services;
    }
}
