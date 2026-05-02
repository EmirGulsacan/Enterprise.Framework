namespace Enterprise.Framework.Infrastructure.BackgroundJobs;

using System.Text.Json;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Events;
using Enterprise.Framework.Domain.Entities;
using Enterprise.Framework.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class OutboxProcessorBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorBackgroundService> _logger;

    public OutboxProcessorBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxMessages(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task ProcessOutboxMessages(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var messages = await dbContext.GetDbSet<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(stoppingToken);

        if (!messages.Any())
            return;

        foreach (var message in messages)
        {
            try
            {
                var type = Type.GetType(message.Type);
                if (type == null)
                {
                    _logger.LogWarning("Domain Event type {Type} not found.", message.Type);
                    continue;
                }

                var domainEvent = JsonSerializer.Deserialize(message.Content, type);
                if (domainEvent == null) continue;

                // Domain olayını uygun bildirime dönüştür: DomainEventNotification<T>
                var notificationType = typeof(DomainEventNotification<>).MakeGenericType(type);
                var notification = Activator.CreateInstance(notificationType, domainEvent);

                if (notification != null)
                {
                    await mediator.Publish(notification, stoppingToken);
                }

                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox message {MessageId}", message.Id);
                message.Error = ex.ToString();
            }
        }

        await dbContext.SaveChangesAsync(stoppingToken);
    }
}
