using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

namespace Fullerene.Manager.Application.Extensions.Mapping.ArtifactDataRecords;

public static class AssetsArtifactSplitDataRecordExtensions
{
    public static ArtifactDataRecordDto ToDto(this AssetsArtifactSplitDataRecord assetsArtifactSplitDataRecord)
    {
        var dto = (assetsArtifactSplitDataRecord as ArtifactSplitDataRecord).ToDto();

        dto.DeliveryType = assetsArtifactSplitDataRecord.DeliveryType;
        dto.AssetModuleType = assetsArtifactSplitDataRecord.AssetModuleType;
        dto.TextureCompressionFormat = assetsArtifactSplitDataRecord.TextureCompressionFormat;
        dto.LanguageTargeting = assetsArtifactSplitDataRecord.LanguageTargeting;

        return dto;
    }
}