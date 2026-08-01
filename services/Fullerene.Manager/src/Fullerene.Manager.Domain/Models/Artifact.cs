using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Domain.Models;

public sealed class Artifact
{
    public required Guid Id { get; init; }
    public required Guid BuildWorkflowId { get; init; }
    public required Guid ArtifactDataRecordId { get; init; }

    public required bool IsSigned { get; init; }
    public required StorageFileData FileData { get; init; }
    public required StorageFileData? IdSigFileData { get; init; }

    public BuildWorkflow BuildWorkflow { get; init; }
    public ArtifactDataRecord ArtifactDataRecord { get; init; }
}