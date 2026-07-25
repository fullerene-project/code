using System.Diagnostics.CodeAnalysis;

namespace Fullerene.Manager.Domain.Models.WorkflowEvents;

public sealed class WorkflowEvent
{
    public required Guid Id { get; init; }
    public Guid BuildWorkflowId { get; private set; }
    public DateTimeOffset DateTimeOffset { get; private set; }
    public WorkflowEventType EventType { get; private set; }

    public BuildWorkflow BuildWorkflow { get; set; }

    public required WorkflowEventPayload Payload
    {
        get;
        init
        {
            field = value;
            EventType = value.EventType;
        }
    }

    [SetsRequiredMembers]
    private WorkflowEvent(Guid id, Guid buildWorkflowId,
        DateTimeOffset dateTimeOffset, WorkflowEventPayload payload)
    {
        Id = id;
        BuildWorkflowId = buildWorkflowId;
        DateTimeOffset = dateTimeOffset;
        EventType = payload.EventType;
        Payload = payload;
    }

    public static WorkflowEvent CreateNew(Guid buildWorkflowId,
        DateTimeOffset dateTimeOffset, WorkflowEventPayload payload)
    {
        return new WorkflowEvent(
            id: Guid.CreateVersion7(),
            buildWorkflowId: buildWorkflowId,
            dateTimeOffset: dateTimeOffset,
            payload: payload);
    }
}