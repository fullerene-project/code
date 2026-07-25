using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Dtos;

public sealed class ArtifactDto
{
    public required Guid Id { get; init; }
    public required Guid BuildWorkflowId { get; init; }

    public required bool IsSigned { get; init; }
    public required StorageFileData FileData { get; init; }
    public required StorageFileData? IdSigFileData { get; init; }

    public required ArtifactDataRecordDto Meta { get; init; }
}