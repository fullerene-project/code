namespace Fullerene.Manager.Application.Dtos;

public sealed record CombinedNixAppPackageIdentifier(
    Guid NixPackageRepoId,
    string AndroidApplicationId,
    string NixPackageName);