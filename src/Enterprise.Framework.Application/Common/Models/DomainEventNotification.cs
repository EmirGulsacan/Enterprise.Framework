namespace Enterprise.Framework.Application.Common.Models;

using Enterprise.Framework.Domain.Common;

using MediatR;



public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : BaseEvent {

public TDomainEvent DomainEvent  { get; }DomainEventNotification(TDomainEvent domainEvent) {
    
DomainEvent = domainEvent;

    }
}



