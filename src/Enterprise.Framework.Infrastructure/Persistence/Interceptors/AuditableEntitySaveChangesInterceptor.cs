namespace Enterprise.Framework.Infrastructure.Persistence.Interceptors;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Diagnostics;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Common;

using Keycloak.Identity.Shared.Interfaces;

using Microsoft.Extensions.DependencyInjection;



public sealed class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor {

private readonly IServiceProvider _serviceProvider;

    private readonly IDateTimeProvider _dateTime;

    public AuditableEntitySaveChangesInterceptor( {
        IServiceProvider serviceProvider,
        IDateTimeProvider dateTime) {
    
_serviceProvider = serviceProvider;

        _dateTime = dateTime;

    

 override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result) {
    
UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);

    

 override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
    
UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);

    

 void UpdateEntities(DbContext? context) {
    
if (context == null) return;

        using var scope = _serviceProvider.CreateScope();

        var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>()) {
        
if (entry.State == EntityState.Added) {
            
entry.Entity.CreatedAtUtc = _dateTime.UtcNow;

                entry.Entity.CreatedBy = currentUserService.UserId;

            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities()) {
            
entry.Entity.LastModifiedAtUtc = _dateTime.UtcNow;

                entry.Entity.LastModifiedBy = currentUserService.UserId;

            }

        }

    }



 static class Extensions {

public static bool HasChangedOwnedEntities(this Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry) => {
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));

}


}

}

}

}

}
}



