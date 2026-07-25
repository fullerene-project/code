using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

namespace Fullerene.Manager.Application.Extensions.Mapping.ArtifactDataRecords;

public static class FeatureArtifactSplitDataRecordExtensions
{
    public static ArtifactDataRecordDto ToDto(this FeatureArtifactSplitDataRecord featureArtifactSplitDataRecord)
    {
        var dto = (featureArtifactSplitDataRecord as MasterArtifactSplitDataRecord).ToDto();

        return dto;
    }
}