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
        var artifact = await context.Artifacts
            .FirstOrDefaultAsync(x => x.Id == @event.ArtifactId, ct);

        if (artifact is null)
        {
            logger.LogWarning("No artifact with id: \"{ArtifactId}\" found", @event.ArtifactId);
            return;
        }

        var workflowEvent = WorkflowEvent.CreateNew(
            buildWorkflowId: artifact.BuildWorkflowId,
            dateTimeOffset: @event.PublishDateTimeOffset,
            payload: new SigningFailedWorkflowEventPayload
            {
                ArtifactId = @event.ArtifactId,
                ErrorText = @event.ErrorText,
            });

        context.WorkflowEvents.Add(workflowEvent);

        await context.SaveChangesAsync(ct);
    }
}