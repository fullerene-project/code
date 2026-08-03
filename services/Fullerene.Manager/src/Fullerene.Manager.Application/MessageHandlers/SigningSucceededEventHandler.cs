using Fullerene.Shared.Contracts.Signing;
using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.WorkflowEvents;
using Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;
using Fullerene.Shared.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fullerene.Manager.Application.MessageHandlers;

public sealed class SigningSucceededEventHandler(
    IApplicationContext context,
    ILogger<SigningSucceededEventHandler> logger)
{
    public async Task Handle(SigningSucceededEvent @event, CancellationToken ct)
    {
        var buildWorkflow = await context.BuildWorkflows
            .Include(buildWorkflow => buildWorkflow.Artifacts)
            .FirstOrDefaultAsync(bw => bw.Id == @event.BuildWorkflowId, ct);

        if (buildWorkflow is null)
        {
            logger.LogWarning("No build workflow with id: \"{BuildWorkflowId}\" found", @event.BuildWorkflowId);
            return;
        }
        
        var signedArtifacts = buildWorkflow.Artifacts
            .Join(
                @event.SignedArtifactsData, 
                x => x.Id, 
                data => data.UnsignedArtifactId, 
                (unsignedArtifact, signedData) => new Artifact 
                { 
                    Id = Guid.CreateVersion7(), 
                    BuildWorkflowId = buildWorkflow.Id, 
                    ArtifactDataRecordId = unsignedArtifact.ArtifactDataRecordId, 
                    IsSigned = true, 
                    FileData = signedData.SignedApkFileData, 
                    IdSigFileData = signedData.SignedApkIdSigFileData 
                })
            .ToArray();

        if (signedArtifacts.Length != buildWorkflow.Artifacts.Count)
        {
            throw new InternalException("The number of signed artifacts does not match the number of unsigned artifacts");
        }
        
        buildWorkflow.SigningFinished(signedArtifacts);
        context.Artifacts.AddRange(signedArtifacts);
        
        var workflowEvent = WorkflowEvent.CreateNew(
            buildWorkflowId: buildWorkflow.Id,
            dateTimeOffset: @event.PublishDateTimeOffset,
            payload: new SigningSucceededWorkflowEventPayload
            {
                SignedArtifactIds = signedArtifacts.Select(art => art.Id).ToArray()
            });

        context.WorkflowEvents.Add(workflowEvent);

        await context.SaveChangesAsync(ct);
    }
}