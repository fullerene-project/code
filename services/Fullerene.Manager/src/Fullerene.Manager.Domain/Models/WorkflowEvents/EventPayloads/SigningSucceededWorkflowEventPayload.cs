namespace Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;

public sealed class SigningSucceededWorkflowEventPayload : WorkflowEventPayload
{
    public override WorkflowEventType EventType => WorkflowEventType.SigningSucceeded;

    public required Guid[] SignedArtifactIds { get; set; }
}