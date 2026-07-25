using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

public sealed class AssetsArtifactSplitDataRecord : ArtifactSplitDataRecord
{
    public required DeliveryType DeliveryType { get; init; }
    public required AssetModuleType AssetModuleType { get; init; }

    public required TextureCompressionFormat? TextureCompressionFormat { get; init; }
    public required string? LanguageTargeting { get; init; }
}