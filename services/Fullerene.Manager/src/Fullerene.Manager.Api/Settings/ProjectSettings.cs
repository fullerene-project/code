using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions;

namespace Fullerene.Manager.Api.Settings;

public sealed class ProjectSettings : ISelfValidatingConfiguration
{
    public required string LicenseTextUrl { get; set; }
    public string? LicenseHtmlUrl { get; set; }
    public required string SourceCodeUrl { get; set; }

    public void ValidateOrThrow()
    {
        ConfigValidationHelper.NotNullOrWhiteSpace(LicenseTextUrl, nameof(LicenseTextUrl));
        ConfigValidationHelper.NotNullOrWhiteSpace(SourceCodeUrl, nameof(SourceCodeUrl));
    }
}