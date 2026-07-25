namespace Fullerene.Shared.Contracts.Signing;

public sealed class SigningFailedEvent : FullereneMessage
{
    public required Guid ArtifactId { get; init; }
    public required string ErrorText { get; init; }
}