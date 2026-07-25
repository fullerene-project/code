namespace Fullerene.Shared.Contracts.Signing;

public sealed class SigningTask : FullereneMessage
{
    public required Guid UnsignedArtifactId { get; init; }
    public required string AndroidAppId { get; init; }
    public required string UnsignedApkStorageKey { get; init; }
}