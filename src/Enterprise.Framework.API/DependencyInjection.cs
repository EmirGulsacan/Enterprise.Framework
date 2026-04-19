namespace Enterprise.Framework.API;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

public static class DependencyInjection {
    public static IServiceCollection AddApiServices(this IServiceCollection services) {
        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Enterprise.Framework API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }
}
