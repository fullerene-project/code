namespace Fullerene.Shared.Contracts.Signing;

public sealed class SigningFailedEvent : FullereneMessage
{
    public required Guid BuildWorkflowId { get; init; }
    public required string ErrorMessage { get; init; }
}