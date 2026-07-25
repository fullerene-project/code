using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

namespace Fullerene.Manager.Application.Extensions.Mapping.ArtifactDataRecords;

public static class AbiArtifactSplitDataRecordExtensions
{
    public static ArtifactDataRecordDto ToDto(this AbiArtifactSplitDataRecord artifactSplitDataRecord)
    {
        var dto = (artifactSplitDataRecord as ArtifactSplitDataRecord).ToDto();

        dto.CpuArchitectures = [artifactSplitDataRecord.CpuArchitecture];

        return dto;
    }
}