using Fullerene.Shared.Contracts.Signing;
using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Domain.Models.WorkflowEvents;
using Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fullerene.Manager.Application.MessageHandlers;

public sealed class SigningFailedEventHandler(
    IApplicationContext context,
    ILogger<SigningFailedEventHandler> logger)
{
    public async Task Handle(SigningFailedEvent @event, CancellationToken ct)
    {
        var buildWorkflow = await context.BuildWorkflows
            .FirstOrDefaultAsync(x => x.Id == @event.BuildWorkflowId, ct);

        if (buildWorkflow is null)
        {
            logger.LogWarning("No build workflow with id: \"{BuildWorkflowId}\" found", @event.BuildWorkflowId);
            return;
        }
        
        buildWorkflow.SigningFailed();

        var workflowEvent = WorkflowEvent.CreateNew(
            buildWorkflowId: buildWorkflow.Id,
            dateTimeOffset: @event.PublishDateTimeOffset,
            payload: new SigningFailedWorkflowEventPayload
            {
                BuildWorkflowId = @event.BuildWorkflowId,
                ErrorMessage = @event.ErrorMessage
            });

        context.WorkflowEvents.Add(workflowEvent);

        await context.SaveChangesAsync(ct);
    }
}