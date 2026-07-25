using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Exceptions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Services.ArtifactMappers;

public sealed class BuildResultToStandaloneUniversalArtifactMapper : IBuildResultToArtifactMapper
{
    public ArtifactType TypeToMap => ArtifactType.StandaloneUniversal;

    public ArtifactDataRecord Map(BuildResultEntry buildResultEntry, Guid buildWorkflowId)
    {
        return new StandaloneUniversalArtifactDataRecord
        {
            Id = Guid.CreateVersion7(),
            ArtifactType = ArtifactType.StandaloneUniversal,
            VersionCode = buildResultEntry.VersionCode,
            MinApiLevel = buildResultEntry.MinApiLevel,
            TargetApiLevel = buildResultEntry.TargetApiLevel,
            CpuArchitectures = buildResultEntry.CpuArchitectures ??
                               throw InvalidBuildResultEntryException.RequiredValueNull(nameof(buildResultEntry.CpuArchitectures))
        };
    }
}