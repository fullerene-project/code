namespace Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;

public sealed class SigningFailedWorkflowEventPayload : WorkflowEventPayload
{
    public override WorkflowEventType EventType => WorkflowEventType.SigningFailed;

    public required Guid BuildWorkflowId { get; set; }
    public required string ErrorMessage { get; set; }
}