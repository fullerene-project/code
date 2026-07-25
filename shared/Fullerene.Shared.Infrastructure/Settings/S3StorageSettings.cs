using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions;

namespace Fullerene.Shared.Infrastructure.Settings;

public sealed class S3StorageSettings : ISelfValidatingConfiguration
{
    public required string InternalUrl { get; init; }
    public string? PublicUrl { get; init; }
    public required string AccessKey { get; init; }
    public required string SecretKey { get; init; }
    public required string Region { get; init; }

    public void ValidateOrThrow()
    {
        ConfigValidationHelper.NotNullOrWhiteSpace(InternalUrl, nameof(InternalUrl));

        ConfigValidationHelper.NotNullOrWhiteSpace(AccessKey, nameof(AccessKey));

        ConfigValidationHelper.NotNullOrWhiteSpace(SecretKey, nameof(SecretKey));

        ConfigValidationHelper.NotNullOrWhiteSpace(Region, nameof(Region));
    }
}