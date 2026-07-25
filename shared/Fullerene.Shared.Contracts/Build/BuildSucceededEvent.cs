namespace Fullerene.Shared.Contracts.Build;

public sealed class BuildSucceededEvent : FullereneMessage
{
    public required Guid BuildWorkflowId { get; init; }
    public required BuildResultManifest Manifest { get; init; }
}