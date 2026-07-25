using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions;

namespace Fullerene.Signer.Infrastructure.Settings;

public sealed class TempSecureDirectorySettings : ISelfValidatingConfiguration
{
    public required string Path { get; init; }

    public void ValidateOrThrow()
    {
        ConfigValidationHelper.NotNullOrWhiteSpace(Path, nameof(Path));
    }
}