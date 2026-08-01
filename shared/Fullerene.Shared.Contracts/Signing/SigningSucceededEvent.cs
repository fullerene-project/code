using Fullerene.Shared.Domain.Models;

namespace Fullerene.Shared.Contracts.Signing;

public sealed class SigningSucceededEvent : FullereneMessage
{
    public required Guid BuildWorkflowId { get; init; }
    public required IEnumerable<SignedArtifactData> SignedArtifactsData { get; init; }
}

public sealed class SignedArtifactData
{
    public required Guid UnsignedArtifactId { get; init; }
    public required StorageFileData SignedApkFileData { get; init; }
    public required StorageFileData SignedApkIdSigFileData { get; init; }
}