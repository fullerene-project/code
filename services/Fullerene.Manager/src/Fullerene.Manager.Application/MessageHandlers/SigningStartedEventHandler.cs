using Fullerene.Shared.Contracts.Signing;
using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Domain.Models.WorkflowEvents;
using Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fullerene.Manager.Application.MessageHandlers;

public sealed class SigningStartedEventHandler(
    IApplicationContext context,
    ILogger<SigningStartedEventHandler> logger)
{
    public async Task Handle(SigningStartedEvent @event, CancellationToken ct)
    {
        var buildWorkflow = await context.BuildWorkflows
            .FirstOrDefaultAsync(bw => bw.Id == @event.BuildWorkflowId, ct);

        if (buildWorkflow is null)
        {
            logger.LogWarning("No workflow found with id: \"{BuildWorkflowId}\"", @event.BuildWorkflowId);
            return;
        }
        
        logger.LogInformation("Signing started. Build workflow id: \"{BuildWorkflowId}\"", @event.BuildWorkflowId);
        
        buildWorkflow.SigningStarted();

        var workflowEvent = WorkflowEvent.CreateNew(
            buildWorkflowId: buildWorkflow.Id,
            dateTimeOffset: @event.PublishDateTimeOffset,
            payload: new SigningStartedWorkflowEventPayload { BuildWorkflowId = @event.BuildWorkflowId });

        context.WorkflowEvents.Add(workflowEvent);

        await context.SaveChangesAsync(ct);
    }
}