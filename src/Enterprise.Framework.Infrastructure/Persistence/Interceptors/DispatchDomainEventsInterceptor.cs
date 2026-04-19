namespace Enterprise.Framework.Infrastructure.Persistence.Interceptors;

using Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;

using Enterprise.Framework.Application.Common.Models;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Diagnostics;



public sealed class DispatchDomainEventsInterceptor : SaveChangesInterceptor {

private readonly IMediator _mediator;

    public DispatchDomainEventsInterceptor(IMediator mediator) {
    
_mediator = mediator;

    

 override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result) {
    
DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();

        return base.SavingChanges(eventData, result);

    

 override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
    
await DispatchDomainEvents(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);

    

 async Task DispatchDomainEvents(DbContext? context) {
    
if (context == null) return;

        var entities = context.ChangeTracker
            .Entries<BaseEntity>() {
            .Where(e => e.Entity.DomainEvents.Any()) {
            .Select(e => e.Entity) {
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents) {
            .ToList();

        entities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents) {
        
var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());

            var notification = Activator.CreateInstance(notificationType, domainEvent);

            if (notification != null) {
            
await _mediator.Publish(notification);

            }

        }

    }

}


}

}

}






}
}
}
}



