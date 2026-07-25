using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Exceptions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Services.ArtifactMappers;

public sealed class BuildResultToBaseArtifactSplitMapper : IBuildResultToArtifactMapper
{
    public ArtifactType TypeToMap => ArtifactType.BaseSplit;

    public ArtifactDataRecord Map(BuildResultEntry buildResultEntry, Guid buildWorkflowId)
    {
        return new BaseArtifactSplitDataRecord
        {
            Id = Guid.CreateVersion7(),
            ArtifactType = ArtifactType.BaseSplit,
            VersionCode = buildResultEntry.VersionCode,
            MinApiLevel = buildResultEntry.MinApiLevel,
            TargetApiLevel = buildResultEntry.TargetApiLevel,
            SplitId = buildResultEntry.SplitId
                      ?? throw InvalidBuildResultEntryException.RequiredValueNull(nameof(buildResultEntry.SplitId)),
            ModuleName = buildResultEntry.ModuleName
                         ?? throw InvalidBuildResultEntryException.RequiredValueNull(nameof(buildResultEntry.ModuleName)),
        };
    }
}