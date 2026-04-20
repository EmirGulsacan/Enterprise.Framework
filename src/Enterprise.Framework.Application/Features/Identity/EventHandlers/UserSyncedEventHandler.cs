namespace Enterprise.Framework.Application.Features.Identity.EventHandlers;

using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Events;
using Enterprise.Framework.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public class UserSyncedEventHandler : INotificationHandler<DomainEventNotification<UserSyncedEvent>>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<UserSyncedEventHandler> _logger;

    public UserSyncedEventHandler(
        IEmailService emailService,
        ILogger<UserSyncedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<UserSyncedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("UserSyncedEvent handled for {Email}", domainEvent.User.Email);

        await _emailService.SendEmailAsync(
            domainEvent.User.Email,
            "Sisteme Hoş Geldiniz",
            $"Merhaba {domainEvent.User.FirstName}, Enterprise.Framework sistemine kaydınız başarıyla tamamlandı.");
    }
}
