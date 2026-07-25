using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions;

namespace Fullerene.Worker.Infrastructure.Settings;

public sealed class PodmanSettings : ISelfValidatingConfiguration
{
    public required string NixVolume { get; init; }
    public required string FileTransferVolume { get; init; }
    public required string NixImage { get; init; }

    public void ValidateOrThrow()
    {
        ConfigValidationHelper.NotNullOrWhiteSpace(NixVolume, nameof(NixVolume));

        ConfigValidationHelper.NotNullOrWhiteSpace(FileTransferVolume, nameof(FileTransferVolume));

        ConfigValidationHelper.NotNullOrWhiteSpace(NixImage, nameof(NixImage));
        ConfigValidationHelper.MatchRegex(NixImage,
            @"^[a-zA-Z0-9.-]+\.[a-zA-Z0-9.-]+(?:/[a-zA-Z0-9._-]+)+\:[a-zA-Z0-9._-]+$", nameof(NixImage));
    }
}