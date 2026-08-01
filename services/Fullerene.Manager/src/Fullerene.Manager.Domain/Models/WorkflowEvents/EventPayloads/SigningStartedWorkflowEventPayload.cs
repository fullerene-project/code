namespace Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;

public sealed class SigningStartedWorkflowEventPayload : WorkflowEventPayload
{
    public override WorkflowEventType EventType => WorkflowEventType.SigningStarted;

    public required Guid BuildWorkflowId { get; set; }
}