using Enterprise.Framework.API;
using Enterprise.Framework.API.Common;
using Enterprise.Framework.API.Middleware;
using Enterprise.Framework.Application;
using Enterprise.Framework.Infrastructure;
using Enterprise.Framework.Infrastructure.Persistence;
using Keycloak.Identity.Shared;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Exceptions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails());

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApiServices();
    builder.Services.AddAuthorization();
    builder.Services.AddSharedIdentity(builder.Configuration);

    builder.Services.PostConfigure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(
        Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters.ValidateAudience = false;
        });

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Log.Information("Initializing database...");
            dbContext.Database.Migrate();
            await DbInitializer.SeedAsync(dbContext);
            Log.Information("Database initialization completed successfully.");

            await Enterprise.Framework.Infrastructure.Identity.RoleInitializer.InitializeAsync(app.Services);
            var seeder = scope.ServiceProvider.GetRequiredService<Enterprise.Framework.Infrastructure.Persistence.PermissionSeeder>();
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while initializing the database.");
        }
    }

    app.UseCors("AllowAll");
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseMiddleware<JitProvisioningMiddleware>();
    app.UseAuthorization();

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Enterprise.Framework API v1"));

    app.MapGet("/", () => Results.Redirect("/swagger"));
    app.MapGet("/health", (HttpContext ctx) =>
        Results.Ok(new { status = "Healthy", traceId = ctx.TraceIdentifier }));

    app.MapAllEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

