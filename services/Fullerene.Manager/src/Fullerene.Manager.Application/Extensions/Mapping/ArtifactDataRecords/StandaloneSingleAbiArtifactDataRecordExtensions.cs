using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

namespace Fullerene.Manager.Application.Extensions.Mapping.ArtifactDataRecords;

public static class StandaloneSingleAbiArtifactDataRecordExtensions
{
    public static ArtifactDataRecordDto ToDto(this StandaloneSingleAbiArtifactDataRecord standaloneSingleAbiArtifactDataRecord)
    {
        var dto = standaloneSingleAbiArtifactDataRecord.MapDefaultValues();

        dto.CpuArchitectures = [standaloneSingleAbiArtifactDataRecord.CpuArchitecture];

        return dto;
    }
}