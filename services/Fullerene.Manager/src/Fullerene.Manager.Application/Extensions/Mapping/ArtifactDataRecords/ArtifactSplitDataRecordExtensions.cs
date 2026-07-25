using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models;

namespace Fullerene.Manager.Application.Extensions.Mapping.ArtifactDataRecords;

public static class ArtifactSplitDataRecordExtensions
{
    public static ArtifactDataRecordDto ToDto(this ArtifactSplitDataRecord artifactSplitDataRecord)
    {
        var dto = artifactSplitDataRecord.MapDefaultValues();

        dto.SplitId = artifactSplitDataRecord.SplitId;
        dto.ModuleName = artifactSplitDataRecord.ModuleName;

        return dto;
    }
}