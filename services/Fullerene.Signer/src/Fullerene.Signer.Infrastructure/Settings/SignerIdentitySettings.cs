using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions;

namespace Fullerene.Signer.Infrastructure.Settings;

public class SignerIdentitySettings : ISelfValidatingConfiguration
{
    public required string CommonName { get; init; }
    public required string Organization { get; init; }

    public void ValidateOrThrow()
    {
        ConfigValidationHelper.NotNullOrWhiteSpace(CommonName, nameof(CommonName));

        ConfigValidationHelper.NotNullOrWhiteSpace(Organization, nameof(Organization));
    }
}