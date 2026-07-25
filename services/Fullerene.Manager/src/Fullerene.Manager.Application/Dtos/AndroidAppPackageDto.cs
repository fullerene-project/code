namespace Fullerene.Manager.Application.Dtos;

public sealed class AndroidAppPackageDto
{
    public required Guid Id { get; init; }
    public required Guid NixPackageRepoId { get; init; }
    public required string NixPackageName { get; init; }
    public required string AndroidApplicationId { get; init; }
    public required bool IsTracked { get; init; }
    public required string AppLogoUrl { get; init; }
    public required string AppName { get; init; }
    public required string AppSummary { get; init; }
    public required string AppDescription { get; init; }
    public required string AppLicense { get; init; }
}