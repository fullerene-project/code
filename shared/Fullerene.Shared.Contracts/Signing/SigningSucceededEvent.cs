using Fullerene.Shared.Domain.Models;

namespace Fullerene.Shared.Contracts.Signing;

public sealed class SigningSucceededEvent : FullereneMessage
{
    public required Guid UnsignedArtifactId { get; set; }
    public required StorageFileData SignedApkFileData { get; set; }
    public required StorageFileData SignedApkIdSigFileData { get; set; }
}