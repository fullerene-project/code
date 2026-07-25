using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Exceptions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Services.ArtifactMappers;

public sealed class BuildResultToStandaloneSingleAbiArtifactMapper : IBuildResultToArtifactMapper
{
    public ArtifactType TypeToMap => ArtifactType.StandaloneSingleAbi;

    public ArtifactDataRecord Map(BuildResultEntry buildResultEntry, Guid buildWorkflowId)
    {
        return new StandaloneSingleAbiArtifactDataRecord
        {
            Id = Guid.CreateVersion7(),
            ArtifactType = ArtifactType.StandaloneSingleAbi,
            VersionCode = buildResultEntry.VersionCode,
            MinApiLevel = buildResultEntry.MinApiLevel,
            TargetApiLevel = buildResultEntry.TargetApiLevel,
            CpuArchitecture = buildResultEntry.SingleCpuArchitecture ??
                               throw InvalidBuildResultEntryException.RequiredValueNull(nameof(buildResultEntry.SingleCpuArchitecture))
        };
    }
}