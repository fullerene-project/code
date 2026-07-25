using System.Text.Json.Serialization;
using Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;

namespace Fullerene.Manager.Domain.Models.WorkflowEvents;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(BuildStartedWorkflowEventPayload), "buildStarted")]
[JsonDerivedType(typeof(BuildFailedWorkflowEventPayload), "buildFailed")]
[JsonDerivedType(typeof(BuildSucceededWorkflowEventPayload), "buildSucceeded")]
[JsonDerivedType(typeof(SigningStartedWorkflowEventPayload), "signingStarted")]
[JsonDerivedType(typeof(SigningFailedWorkflowEventPayload), "signingFailed")]
[JsonDerivedType(typeof(SigningSucceededWorkflowEventPayload), "signingSucceeded")]
public abstract class WorkflowEventPayload
{
    [JsonIgnore]
    public abstract WorkflowEventType EventType { get; }
};