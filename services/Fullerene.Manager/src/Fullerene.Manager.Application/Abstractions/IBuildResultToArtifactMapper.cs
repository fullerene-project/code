using Fullerene.Manager.Domain.Models;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Abstractions;

public interface IBuildResultToArtifactMapper
{
    public ArtifactType TypeToMap { get; }

    public ArtifactDataRecord Map(BuildResultEntry buildResultEntry, Guid buildWorkflowId);
}