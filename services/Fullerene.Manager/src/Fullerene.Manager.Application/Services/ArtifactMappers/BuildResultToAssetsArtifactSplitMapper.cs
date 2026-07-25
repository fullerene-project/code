using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Exceptions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Services.ArtifactMappers;

public sealed class BuildResultToAssetsArtifactSplitMapper : IBuildResultToArtifactMapper
{
    public ArtifactType TypeToMap => ArtifactType.AssetsSplit;

    public ArtifactDataRecord Map(BuildResultEntry buildResultEntry, Guid buildWorkflowId)
    {
        return new AssetsArtifactSplitDataRecord
        {
            Id = Guid.CreateVersion7(),
            ArtifactType = ArtifactType.AssetsSplit,
            VersionCode = buildResultEntry.VersionCode,
            MinApiLevel = buildResultEntry.MinApiLevel,
            TargetApiLevel = buildResultEntry.TargetApiLevel,
            SplitId = buildResultEntry.SplitId ??
                      throw InvalidBuildResultEntryException.RequiredValueNull(nameof(buildResultEntry.SplitId)),
            ModuleName = buildResultEntry.ModuleName ??
                         throw InvalidBuildResultEntryException.RequiredValueNull(nameof(buildResultEntry.ModuleName)),
            DeliveryType = buildResultEntry.DeliveryType ??
                           throw InvalidBuildResultEntryException.RequiredValueNull(nameof(buildResultEntry.DeliveryType)),
            AssetModuleType = buildResultEntry.AssetModuleType ??
                              throw InvalidBuildResultEntryException.RequiredValueNull(nameof(buildResultEntry.AssetModuleType)),
            TextureCompressionFormat = buildResultEntry.TextureCompressionFormat,
            LanguageTargeting = buildResultEntry.LanguageTargeting
        };
    }
}