namespace Enterprise.Framework.Application;

using FluentValidation;

using Enterprise.Framework.Application.Common.Behaviors;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Application.Common.Rules;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;



public static class DependencyInjection {

public static IServiceCollection AddApplication(this IServiceCollection services) {
    
services.AddAutoMapper(cfg =>
        
cfg.AddMaps(Assembly.GetExecutingAssembly());

        }
);

        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        services.AddScoped<IBusinessRuleEngine, BusinessRuleEngine>();

        return services;
}



