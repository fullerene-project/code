using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions;

namespace Fullerene.Shared.Infrastructure.Settings;

public abstract class S3BucketSettings : ISelfValidatingConfiguration
{
    public required string BucketName { get; init; }

    public void ValidateOrThrow()
    {
        ConfigValidationHelper.NotNullOrWhiteSpace(BucketName, nameof(BucketName));
    }
}