using Fullerene.Shared.Common.Abstractions.Messaging;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Shared.Contracts.Signing;
using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.WorkflowEvents;
using Fullerene.Manager.Domain.Models.WorkflowEvents.EventPayloads;
using Fullerene.Shared.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fullerene.Manager.Application.MessageHandlers;

public sealed class BuildSucceededEventHandler(
    IApplicationContext context,
    IBuildResultToArtifactMapperService artifactMapperService,
    ILogger<BuildSucceededEventHandler> logger,
    ITaskPublisher taskPublisher)
{
    public async Task Handle(BuildSucceededEvent @event, CancellationToken ct)
    {
        logger.LogInformation("Build succeeded. Workflow id: \"{BuildWorkflowId}\"", @event.BuildWorkflowId);

        var workflow = await context.BuildWorkflows
            .Include(x => x.AndroidAppPackageVersion)
            .FirstOrDefaultAsync(x => x.Id == @event.BuildWorkflowId, ct);

        if (workflow is null)
        {
            logger.LogWarning("No workflow with id {BuildWorkflowId} found", @event.BuildWorkflowId);
            return;
        }

        var artifacts = new List<Artifact>(@event.Manifest.Entries.Count);

        foreach (var buildResult in @event.Manifest.Entries)
        {
            var artifactDataRecord = artifactMapperService.Map(buildResult, @event.BuildWorkflowId);

            context.ArtifactDataRecords.Add(artifactDataRecord);

            var artifact = new Artifact
            {
                Id = Guid.CreateVersion7(),
                BuildWorkflowId = workflow.Id,
                IsSigned = false,
                FileData = new StorageFileData
                {
                    FileName = buildResult.FileName,
                    FileSha256 = buildResult.FileSha256,
                    FileSizeBytes = buildResult.FileSizeBytes,
                    FileStorageKey = buildResult.UnsignedApkStorageKey
                },
                IdSigFileData = null,
                ArtifactDataRecordId = artifactDataRecord.Id,
                ArtifactDataRecord = artifactDataRecord
            };
            
            context.Artifacts.Add(artifact);
            artifacts.Add(artifact);
        }
        
        workflow.BuildFinished(artifacts);

        var workflowEvent = WorkflowEvent.CreateNew(
            buildWorkflowId: @event.BuildWorkflowId,
            dateTimeOffset: @event.PublishDateTimeOffset,
            payload: new BuildSucceededWorkflowEventPayload
            {
                ArtifactIds = artifacts.Select(art => art.Id).ToArray()
            });

        context.WorkflowEvents.Add(workflowEvent);
        
        await taskPublisher.PublishTaskAsync(new SigningTask
        {
            BuildWorkflowId = workflow.Id,
            AndroidApplicationId = workflow.AndroidAppPackageVersion.AndroidApplicationId,
            UnsignedArtifactsData = artifacts.Select(art => new UnsignedArtifactData
            {
                UnsignedArtifactId = art.Id,
                UnsignedArtifactStorageKey = art.FileData.FileStorageKey
            }),
            PublishDateTimeOffset = DateTimeOffset.UtcNow,
        }, ct);

        await context.SaveChangesAsync(ct);
    }
}