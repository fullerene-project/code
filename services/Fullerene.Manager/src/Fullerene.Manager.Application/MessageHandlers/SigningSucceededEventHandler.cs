using Fullerene.Shared.Contracts.Signing;
using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.WorkflowEvents;
using Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fullerene.Manager.Application.MessageHandlers;

public sealed class SigningSucceededEventHandler(
    IApplicationContext context,
    ILogger<SigningSucceededEventHandler> logger)
{
    public async Task Handle(SigningSucceededEvent @event, CancellationToken ct)
    {
        var artifact = await context.Artifacts
            .FirstOrDefaultAsync(x => x.Id == @event.UnsignedArtifactId, ct);

        if (artifact is null)
        {
            logger.LogWarning("No artifact with id: \"{ArtifactId}\" found", @event.UnsignedArtifactId);
            return;
        }

        var signedArtifact = new Artifact
        {
            Id = Guid.CreateVersion7(),
            ArtifactDataRecordId = artifact.ArtifactDataRecordId,
            BuildWorkflowId = artifact.BuildWorkflowId,
            IsSigned = true,
            FileData = @event.SignedApkFileData,
            IdSigFileData = @event.SignedApkIdSigFileData
        };

        context.Artifacts.Add(signedArtifact);

        var workflowEvent = WorkflowEvent.CreateNew(
            buildWorkflowId: artifact.BuildWorkflowId,
            dateTimeOffset: @event.PublishDateTimeOffset,
            payload: new SigningSucceededWorkflowEventPayload { SignedArtifactId = signedArtifact.Id });

        context.WorkflowEvents.Add(workflowEvent);

        await context.SaveChangesAsync(ct);
    }
}