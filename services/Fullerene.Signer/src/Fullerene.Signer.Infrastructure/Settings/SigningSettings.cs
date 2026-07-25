using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions;

namespace Fullerene.Signer.Infrastructure.Settings;

public sealed class SigningSettings : ISelfValidatingConfiguration
{
    public required string MasterSeedBase64 { get; init; }

    public void ValidateOrThrow()
    {
        ConfigValidationHelper.NotNullOrWhiteSpace(MasterSeedBase64, nameof(MasterSeedBase64));
        ConfigValidationHelper.LengthBetweenIncluded(MasterSeedBase64, 64, 1024, nameof(MasterSeedBase64.Length));
    }
}