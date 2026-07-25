using Fullerene.Shared.Common.Abstractions.Messaging;
using Fullerene.Shared.Contracts;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Fullerene.Shared.Infrastructure.Services;

public sealed class RabbitMqTaskPublisher(
    IMessageBus bus,
    ILogger<RabbitMqTaskPublisher> logger) : ITaskPublisher
{
    public async Task PublishTaskAsync<TTask>(TTask task, CancellationToken ct) where TTask : FullereneMessage
    {
        task.PublishDateTimeOffset = DateTimeOffset.UtcNow;
        await bus.PublishAsync(task);
        logger.LogInformation($"Task {typeof(TTask).Name} published");
    }
}