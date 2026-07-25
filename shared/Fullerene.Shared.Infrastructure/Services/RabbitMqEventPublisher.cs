using Fullerene.Shared.Common.Abstractions.Messaging;
using Fullerene.Shared.Contracts;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Fullerene.Shared.Infrastructure.Services;

public sealed class RabbitMqEventPublisher(
    IMessageBus bus,
    ILogger<RabbitMqEventPublisher> logger) : IEventPublisher
{
    public async Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : FullereneMessage
    {
        @event.PublishDateTimeOffset = DateTimeOffset.UtcNow;
        await bus.PublishAsync(@event);
        logger.LogInformation($"Event {typeof(TEvent).Name} published");
    }
}