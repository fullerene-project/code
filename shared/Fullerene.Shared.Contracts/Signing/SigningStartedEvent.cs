namespace Fullerene.Shared.Contracts.Signing;

public sealed class SigningStartedEvent : FullereneMessage
{
    public required Guid BuildWorkflowId { get; init; }
}