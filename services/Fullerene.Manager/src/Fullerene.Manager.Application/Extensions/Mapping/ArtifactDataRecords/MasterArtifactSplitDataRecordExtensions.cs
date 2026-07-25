using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models;

namespace Fullerene.Manager.Application.Extensions.Mapping.ArtifactDataRecords;

public static class MasterArtifactSplitDataRecordExtensions
{
    public static ArtifactDataRecordDto ToDto(this MasterArtifactSplitDataRecord masterArtifactSplitDataRecord)
    {
        var dto = (masterArtifactSplitDataRecord as ArtifactSplitDataRecord).ToDto();

        return dto;
    }
}