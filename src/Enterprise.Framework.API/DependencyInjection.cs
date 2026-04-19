namespace Enterprise.Framework.API;

using Microsoft.OpenApi.Models;



public static class DependencyInjection {

public static IServiceCollection AddApiServices(this IServiceCollection services) {
    
services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        
options.SwaggerDoc("v1", new OpenApiInfo 
Title = "Enterprise.Framework API", Version = "v1" }
);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            
Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer 
token}
\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
}



