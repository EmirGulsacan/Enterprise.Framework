namespace Enterprise.Framework.Infrastructure.Persistence.Interceptors;

using System.Text.Json;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public sealed class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        StoreDomainEventsAsOutboxMessages(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        StoreDomainEventsAsOutboxMessages(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void StoreDomainEventsAsOutboxMessages(DbContext? context)
    {
        if (context == null) return;

        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ForEach(e => e.ClearDomainEvents());

        var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage
        {
            Type = domainEvent.GetType().AssemblyQualifiedName ?? domainEvent.GetType().Name,
            Content = JsonSerializer.Serialize((object)domainEvent),
            OccurredOnUtc = DateTime.UtcNow
        }).ToList();

        context.Set<OutboxMessage>().AddRange(outboxMessages);
    }
}
