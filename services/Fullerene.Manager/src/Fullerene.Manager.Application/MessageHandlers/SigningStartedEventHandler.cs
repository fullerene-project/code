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
        logger.LogInformation("Signing started. Unsigned artifact id: \"{UnsignedArtifactId}\"", @event.UnsignedArtifactId);

        var workflowId = await context.BuildWorkflows
            .Where(x => x.Artifacts.Any(y => y.Id == @event.UnsignedArtifactId))
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        if (workflowId == Guid.Empty)
        {
            logger.LogWarning("No workflow found for unsigned artifact with id: \"{UnsignedArtifactId}\"", @event.UnsignedArtifactId);
            return;
        }

        var workflowEvent = WorkflowEvent.CreateNew(
            buildWorkflowId: workflowId,
            dateTimeOffset: @event.PublishDateTimeOffset,
            payload: new SigningStartedWorkflowEventPayload { UnsignedArtifactId = @event.UnsignedArtifactId });

        context.WorkflowEvents.Add(workflowEvent);

        await context.SaveChangesAsync(ct);
    }
}