using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions;

namespace Fullerene.Shared.Infrastructure.Settings;

public abstract class RabbitMqSubscriptionSettings : ISelfValidatingConfiguration
{
    public required string QueueName { get; init; }
    public required ushort ConcurrencyLimit { get; init; } = 1;

    public void ValidateOrThrow()
    {
        ConfigValidationHelper.NotNullOrWhiteSpace(QueueName, nameof(QueueName));
    }
}