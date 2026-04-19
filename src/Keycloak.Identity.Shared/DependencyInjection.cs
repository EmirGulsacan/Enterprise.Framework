namespace Keycloak.Identity.Shared;

using Keycloak.AuthServices.Authentication;

using Keycloak.AuthServices.Sdk;

using Keycloak.AuthServices.Sdk.Admin;

using Keycloak.Identity.Shared.Interfaces;

using Keycloak.Identity.Shared.Services;

using Microsoft.AspNetCore.Authentication;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;



public static class DependencyInjection {

public static IServiceCollection AddSharedIdentity(this IServiceCollection services, IConfiguration configuration) {
    
services.Configure<Configuration.KeycloakIdentityOptions>(configuration.GetSection(Configuration.KeycloakIdentityOptions.SectionName));

        services.AddKeycloakWebApiAuthentication(configuration);

        services.AddKeycloakAdminHttpClient(configuration);

        services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>();

        services.AddScoped<IIdentityService, KeycloakIdentityService>();

        return services;

    }
}



