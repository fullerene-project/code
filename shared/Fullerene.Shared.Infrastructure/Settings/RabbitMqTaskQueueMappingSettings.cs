using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions;

namespace Fullerene.Shared.Infrastructure.Settings;

public sealed class RabbitMqTaskQueueMappingSettings : ISelfValidatingConfiguration
{
    public required Dictionary<string, string> QueueNameByTaskName { get; init; }

    public void ValidateOrThrow()
    {
        foreach (var pair in QueueNameByTaskName)
        {
            ConfigValidationHelper.NotNullOrWhiteSpace(pair.Key, $"one of {nameof(QueueNameByTaskName)} keys");
            ConfigValidationHelper.NotNullOrWhiteSpace(pair.Value, $"{nameof(QueueNameByTaskName)}.{pair.Key}");
        }
    }
}