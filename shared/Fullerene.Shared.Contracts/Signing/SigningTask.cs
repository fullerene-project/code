namespace Fullerene.Shared.Contracts.Signing;

public sealed class SigningTask : FullereneMessage
{
    public required Guid BuildWorkflowId { get; init; }
    public required string AndroidApplicationId { get; init; }
    public required IEnumerable<UnsignedArtifactData> UnsignedArtifactsData { get; init; }
}

public sealed class UnsignedArtifactData
{
    public required Guid UnsignedArtifactId { get; init; }
    public required string UnsignedArtifactStorageKey { get; init; }
}