using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Application.Extensions.Mapping.ArtifactDataRecords;
using Fullerene.Manager.Domain.Models;

namespace Fullerene.Manager.Application.Extensions.Mapping;

public static class ArtifactExtensions
{
    public static ArtifactDto ToDto(this Artifact artifact)
    {
        return new ArtifactDto
        {
            Id = artifact.Id,
            BuildWorkflowId = artifact.BuildWorkflowId,
            IsSigned = artifact.IsSigned,
            FileData = artifact.FileData,
            IdSigFileData = artifact.IdSigFileData,
            Meta = artifact.ArtifactDataRecord.ToDto()
        };
    }
}