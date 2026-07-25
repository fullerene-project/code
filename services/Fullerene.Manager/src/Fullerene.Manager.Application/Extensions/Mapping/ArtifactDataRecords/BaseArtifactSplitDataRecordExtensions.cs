using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

namespace Fullerene.Manager.Application.Extensions.Mapping.ArtifactDataRecords;

public static class BaseArtifactSplitDataRecordExtensions
{
    public static ArtifactDataRecordDto ToDto(this BaseArtifactSplitDataRecord baseArtifactSplitDataRecord)
    {
        var dto = (baseArtifactSplitDataRecord as MasterArtifactSplitDataRecord).ToDto();

        return dto;
    }
}