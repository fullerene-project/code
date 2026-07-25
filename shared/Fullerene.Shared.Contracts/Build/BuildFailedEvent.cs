namespace Fullerene.Shared.Contracts.Build;

public sealed class BuildFailedEvent : FullereneMessage
{
    public required Guid BuildWorkflowId { get; init; }
    public required string ErrorText { get; init; }
}