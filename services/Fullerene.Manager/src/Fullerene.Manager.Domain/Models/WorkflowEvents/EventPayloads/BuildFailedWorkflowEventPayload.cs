namespace Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;

public sealed class BuildFailedWorkflowEventPayload : WorkflowEventPayload
{
    public override WorkflowEventType EventType => WorkflowEventType.BuildFailed;

    public required string ErrorText { get; set; }
}