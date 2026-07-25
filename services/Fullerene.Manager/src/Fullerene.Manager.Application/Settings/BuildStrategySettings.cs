using Fullerene.Manager.Application.Util;
using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions;
using Fullerene.Shared.Common.Exceptions;
using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Settings;

public sealed class BuildStrategySettings : ISelfValidatingConfiguration
{
    public required BuildStrategy BuildStrategy { get; init; }
    public required Dictionary<ReleaseChannel, int>? BuildLatestFromChannels { get; init; }

    public void ValidateOrThrow()
    {
        ConfigValidationHelper.NotNullOrWhiteSpace(BuildStrategy.ToString(), nameof(BuildStrategy));

        if (BuildLatestFromChannels is null ||
            BuildLatestFromChannels.ToArray().Length != Enum.GetValues(typeof(ReleaseChannel)).Length)
            throw new AppConfigurationException(
                $"\"{nameof(BuildLatestFromChannels)}\" section required for every release channel type.");
    }
}