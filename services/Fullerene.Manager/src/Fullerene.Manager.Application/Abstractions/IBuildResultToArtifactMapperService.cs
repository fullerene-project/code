using Fullerene.Manager.Domain.Models;
using Fullerene.Shared.Contracts.Build;

namespace Fullerene.Manager.Application.Abstractions;

public interface IBuildResultToArtifactMapperService
{
    ArtifactDataRecord Map(BuildResultEntry buildResultEntry, Guid buildWorkflowId);
}