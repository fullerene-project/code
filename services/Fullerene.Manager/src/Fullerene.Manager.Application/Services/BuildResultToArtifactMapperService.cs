using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Shared.Domain.Exceptions;
using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Services;

public sealed class BuildResultToArtifactMapperService : IBuildResultToArtifactMapperService
{
    private readonly IDictionary<ArtifactType, IBuildResultToArtifactMapper> _artifactMappers;

    public BuildResultToArtifactMapperService(IEnumerable<IBuildResultToArtifactMapper> mappers)
    {
        _artifactMappers = mappers.ToDictionary(x => x.TypeToMap);
    }

    public ArtifactDataRecord Map(BuildResultEntry buildResultEntry, Guid buildWorkflowId)
    {
        if (_artifactMappers.TryGetValue(buildResultEntry.ArtifactType, out var mapper))
        {
            return mapper.Map(buildResultEntry, buildWorkflowId);
        }
        throw new InternalException($"No artifact mapper for type: \"{buildResultEntry.ArtifactType}\" found");
    }
}