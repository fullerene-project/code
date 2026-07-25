using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

namespace Fullerene.Manager.Application.Extensions.Mapping.ArtifactDataRecords;

public static class ArtifactDataRecordExtensions
{
    public static ArtifactDataRecordDto ToDto(this ArtifactDataRecord artifactDataRecord)
    {
        return artifactDataRecord switch
        {
            AbiArtifactSplitDataRecord abi => abi.ToDto(),
            AssetsArtifactSplitDataRecord assets => assets.ToDto(),
            LanguageArtifactSplitDataRecord language => language.ToDto(),
            DensityArtifactSplitDataRecord density => density.ToDto(),
            FeatureArtifactSplitDataRecord feature => feature.ToDto(),
            StandaloneSingleAbiArtifactDataRecord standaloneSingleAbi => standaloneSingleAbi.ToDto(),
            StandaloneUniversalArtifactDataRecord standaloneUniversal => standaloneUniversal.ToDto(),
            BaseArtifactSplitDataRecord baseRecord => baseRecord.ToDto(),

            _ => throw new ArgumentOutOfRangeException(
                nameof(artifactDataRecord),
                artifactDataRecord,
                "Unknown artifact data record type during mapping to DTO")
        };
    }

    public static ArtifactDataRecordDto MapDefaultValues(this ArtifactDataRecord artifactDataRecord)
    {
        return new ArtifactDataRecordDto
        {
            ArtifactType = artifactDataRecord.ArtifactType,
            VersionCode = artifactDataRecord.VersionCode,
            MinApiLevel = artifactDataRecord.MinApiLevel,
            TargetApiLevel = artifactDataRecord.TargetApiLevel
        };
    }
}