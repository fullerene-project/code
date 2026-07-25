using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

namespace Fullerene.Manager.Application.Extensions.Mapping.ArtifactDataRecords;

public static class DensityArtifactSplitDataRecordExtensions
{
    public static ArtifactDataRecordDto ToDto(this DensityArtifactSplitDataRecord densityArtifactSplitDataRecord)
    {
        var dto = (densityArtifactSplitDataRecord as ArtifactSplitDataRecord).ToDto();

        densityArtifactSplitDataRecord.Density.Match(
            onAlias: alias => dto.DensityAlias = alias,
            onDpi: dpi => dto.DensityDpi = dpi);

        return dto;
    }
}