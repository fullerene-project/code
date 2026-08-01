using Fullerene.Shared.Contracts.Build;
using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Domain.Models.WorkflowEvents;
using Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fullerene.Manager.Application.MessageHandlers;

public sealed class BuildFailedEventHandler(
    IApplicationContext context,
    ILogger<BuildFailedEventHandler> logger)
{
    public async Task Handle(BuildFailedEvent @event, CancellationToken ct)
    {
        logger.LogInformation("Build failed. Workflow id: \"{BuildWorkflowId}\", ErrorMessage: \"{ErrorText}\"", @event.BuildWorkflowId, @event.ErrorText);

        var workflow = await context.BuildWorkflows
            .FirstOrDefaultAsync(x => x.Id == @event.BuildWorkflowId, ct);

        if (workflow is null)
        {
            logger.LogWarning("No workflow with id {BuildWorkflowId} found", @event.BuildWorkflowId);
            return;
        }
        
        workflow.BuildFailed();

        var workflowEvent = WorkflowEvent.CreateNew(
            buildWorkflowId: @event.BuildWorkflowId,
            dateTimeOffset: @event.PublishDateTimeOffset,
            payload: new BuildFailedWorkflowEventPayload { ErrorText = @event.ErrorText });

        context.WorkflowEvents.Add(workflowEvent);

        await context.SaveChangesAsync(ct);
    }
}