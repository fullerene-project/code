using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

namespace Fullerene.Manager.Application.Extensions.Mapping.ArtifactDataRecords;

public static class StandaloneUniversalArtifactDataRecordExtensions
{
    public static ArtifactDataRecordDto ToDto(this StandaloneUniversalArtifactDataRecord standaloneUniversalArtifactDataRecord)
    {
        var dto = standaloneUniversalArtifactDataRecord.MapDefaultValues();

        dto.CpuArchitectures = standaloneUniversalArtifactDataRecord.CpuArchitectures;

        return dto;
    }
}