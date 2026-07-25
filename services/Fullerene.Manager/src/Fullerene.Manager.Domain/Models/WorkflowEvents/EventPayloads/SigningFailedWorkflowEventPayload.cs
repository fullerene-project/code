namespace Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;

public sealed class SigningFailedWorkflowEventPayload : WorkflowEventPayload
{
    public override WorkflowEventType EventType => WorkflowEventType.SigningFailed;

    public required Guid ArtifactId { get; set; }
    public required string ErrorText { get; set; }
}