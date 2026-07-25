namespace Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;

public sealed class BuildStartedWorkflowEventPayload : WorkflowEventPayload
{
    public override WorkflowEventType EventType => WorkflowEventType.BuildStarted;
}