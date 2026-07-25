using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions;

namespace Fullerene.Shared.Infrastructure.Settings;

public sealed class RabbitMqConnectionSettings : ISelfValidatingConfiguration
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string User { get; init; }
    public required string Password { get; init; }

    public void ValidateOrThrow()
    {
        ConfigValidationHelper.NotNullOrWhiteSpace(Host, nameof(Host));

        ConfigValidationHelper.ValueBetweenIncluded(Port, 1, 65535, nameof(Port));

        ConfigValidationHelper.NotNullOrWhiteSpace(User, nameof(User));

        ConfigValidationHelper.NotNullOrWhiteSpace(Password, nameof(Password));
    }
}