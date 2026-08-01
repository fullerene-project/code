using Fullerene.Shared.Contracts.Build;
using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Domain.Models.WorkflowEvents;
using Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fullerene.Manager.Application.MessageHandlers;

public sealed class BuildStartedEventHandler(
    IApplicationContext context,
    ILogger<BuildFailedEventHandler> logger)
{
    public async Task Handle(BuildStartedEvent @event, CancellationToken ct)
    {
        logger.LogInformation("Build started. Workflow id: \"{BuildWorkflowId}\"", @event.BuildWorkflowId);

        var workflow = await context.BuildWorkflows
            .FirstOrDefaultAsync(x => x.Id == @event.BuildWorkflowId, ct);

        if (workflow is null)
        {
            logger.LogWarning("No workflow with id {BuildWorkflowId} found", @event.BuildWorkflowId);
            return;
        }
        
        workflow.BuildStarted();

        var workflowEvent = WorkflowEvent.CreateNew(
            buildWorkflowId: @event.BuildWorkflowId,
            dateTimeOffset: @event.PublishDateTimeOffset,
            payload: new BuildStartedWorkflowEventPayload());

        context.WorkflowEvents.Add(workflowEvent);

        await context.SaveChangesAsync(ct);
    }
}