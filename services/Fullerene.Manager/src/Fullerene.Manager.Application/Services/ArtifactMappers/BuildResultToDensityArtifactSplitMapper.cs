using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Exceptions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Services.ArtifactMappers;

public sealed class BuildResultToDensityArtifactSplitMapper : IBuildResultToArtifactMapper
{
    public ArtifactType TypeToMap => ArtifactType.DensitySplit;

    public ArtifactDataRecord Map(BuildResultEntry buildResultEntry, Guid buildWorkflowId)
    {
        return new DensityArtifactSplitDataRecord
        {
            Id = Guid.CreateVersion7(),
            ArtifactType = ArtifactType.DensitySplit,
            VersionCode = buildResultEntry.VersionCode,
            MinApiLevel = buildResultEntry.MinApiLevel,
            TargetApiLevel = buildResultEntry.TargetApiLevel,
            SplitId = buildResultEntry.SplitId ??
                      throw InvalidBuildResultEntryException.RequiredValueNull(nameof(buildResultEntry.SplitId)),
            ModuleName = buildResultEntry.ModuleName ??
                         throw InvalidBuildResultEntryException.RequiredValueNull(nameof(buildResultEntry.ModuleName)),
            Density = ScreenDensity.FromBoth(buildResultEntry.DensityDpi, buildResultEntry.DensityAlias)
        };
    }
}