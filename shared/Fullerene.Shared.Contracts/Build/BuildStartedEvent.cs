namespace Fullerene.Shared.Contracts.Build;

public sealed class BuildStartedEvent : FullereneMessage
{
    public required Guid BuildWorkflowId { get; init; }
}