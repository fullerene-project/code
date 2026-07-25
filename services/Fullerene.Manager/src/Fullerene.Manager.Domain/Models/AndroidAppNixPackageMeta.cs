using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Domain.Models;

public sealed class AndroidAppNixPackageMeta
{
    public required string PackageName { get; init; }
    public required int NixPackageRevision { get; init; }
    public required string DerivationHash { get; init; }
    public required int BaseVersionCode { get; init; }
    public required string AppVersionString { get; init; }
    public required string AndroidApplicationId { get; init; }
    public required ReleaseChannel ReleaseChannel { get; init; }
    public required DateTimeOffset AppReleaseDate { get; init; }
    public required string AppLogoUrl { get; init; }
    public required string AppName { get; init; }
    public required string AppSummary { get; init; }
    public required string AppDescription { get; init; }
    public required string AppLicense { get; init; }
    public required string? ReleaseNotes { get; init; }
}