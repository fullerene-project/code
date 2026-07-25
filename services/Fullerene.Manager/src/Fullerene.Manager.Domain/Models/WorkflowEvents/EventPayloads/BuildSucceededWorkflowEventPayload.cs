namespace Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;

public sealed class BuildSucceededWorkflowEventPayload : WorkflowEventPayload
{
    public override WorkflowEventType EventType => WorkflowEventType.BuildSucceeded;

    public required ICollection<Guid> ArtifactIds { get; set; }
}