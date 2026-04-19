namespace Enterprise.Framework.Infrastructure;

using Enterprise.Framework.Application.Common.Interfaces;

using Keycloak.Identity.Shared.Interfaces;

using Enterprise.Framework.Application.Common.Caching;

using Enterprise.Framework.Application.Common.Idempotency;

using Enterprise.Framework.Domain.Common;

using Enterprise.Framework.Infrastructure.Caching;

using Enterprise.Framework.Infrastructure.Idempotency;

using Enterprise.Framework.Infrastructure.Persistence;

using Enterprise.Framework.Infrastructure.Persistence.Interceptors;

using Enterprise.Framework.Infrastructure.Persistence.Providers;

using Enterprise.Framework.Infrastructure.Security;

using Enterprise.Framework.Infrastructure.Services;

using Enterprise.Framework.Infrastructure.Services.Communication;

using Enterprise.Framework.Infrastructure.Services.Files;

using Enterprise.Framework.Infrastructure.Identity;

using Microsoft.AspNetCore.Hosting;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;



public static class DependencyInjection {

public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
    
services.AddHttpContextAccessor();

        services.AddMemoryCache();

        services.AddHttpClient<IKeycloakAdminService, KeycloakAdminService>();

        services.AddScoped<PermissionSeeder>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();

        services.AddScoped<IPermissionService, PermissionService>();

        services.AddScoped<IAppCache, MemoryAppCache>();

        services.AddScoped<IIdempotencyStore, InMemoryIdempotencyStore>();

        services.AddTransient<Microsoft.AspNetCore.Authentication.IClaimsTransformation, LocalClaimsTransformation>();

        services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider, Enterprise.Framework.Infrastructure.Security.Authorization.PermissionPolicyProvider>();

        services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, Enterprise.Framework.Infrastructure.Security.Authorization.PermissionAuthorizationHandler>();

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddScoped<DispatchDomainEventsInterceptor>();

        services.AddScoped<IEmailService, SmtpEmailService>();

        services.AddScoped<ISmsService, MockSmsService>();

        services.AddScoped<IFileService, LocalFileService>();

        services.AddScoped<IExcelService, ExcelService>();

        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName) {
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        if (databaseOptions.Provider?.ToLower() == "postgresql") {
        
services.AddSingleton<IDatabaseProvider, PostgreSqlDatabaseProvider>();

        }

        else {
        
services.AddSingleton<IDatabaseProvider, SqlServerDatabaseProvider>();

        }

        services.AddDbContext<AppDbContext>((sp, options) =>
        
var databaseProvider = sp.GetRequiredService<IDatabaseProvider>();

            databaseProvider.Configure(options, databaseOptions.ConnectionString);

        }
);

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        return services;

    }
}



