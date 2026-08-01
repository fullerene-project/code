using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Dtos;

public sealed class AndroidAppPackageVersionDto
{
    public required Guid Id { get; init; }

    public required Guid NixPackageRepoId { get; init; }
    public required string CommitHash { get; init; }
    public required string NixPackageName { get; init; }
    public required string AndroidApplicationId { get; init; }

    public required string AppVersionString { get; init; }
    public required int BaseVersionCode { get; init; }
    public required int NixPackageRevision { get; init; }
    public required string NixDerivationHash { get; init; }
    public required ReleaseChannel ReleaseChannel { get; init; }
    public required DateTimeOffset AppVersionReleaseDate { get; init; }
    public required string AppLogoUrl { get; init; }
    public required string AppName { get; init; }
    public required string AppSummary { get; init; }
    public required string AppDescription { get; init; }
    public required string AppLicense { get; init; }
    public required string? ReleaseNotes { get; init; }
}