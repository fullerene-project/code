using System.Collections.Immutable;
using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions.Messaging;
using Fullerene.Shared.Common.Abstractions.Storage;
using Fullerene.Shared.Contracts.Signing;
using Fullerene.Shared.Domain.Models;
using Fullerene.Signer.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Fullerene.Signer.Application.Services;

public sealed class SigningTaskHandler(
    IApkSigner apkSigner,
    IUnsignedApkStorage unsignedApkStorage,
    ISignedApkStorage signedApkStorage,
    IEventPublisher eventPublisher,
    ILogger<SigningTaskHandler> logger)
{
    public async Task Handle(SigningTask task, CancellationToken ct)
    {
        await eventPublisher.PublishEventAsync(new SigningStartedEvent
        {
            BuildWorkflowId = task.BuildWorkflowId,
        }, ct);

        var workDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(workDir);

        var signingOutputDir = Path.Combine(workDir, "result");

        try
        {
            var downloadedUnsignedArtifacts = 
                (await Task.WhenAll(task.UnsignedArtifactsData.Select(async unsignedArtifactData => 
                {
                    var unsignedArtifactName = Path.GetFileName(unsignedArtifactData.UnsignedArtifactStorageKey);
                    var unsignedArtifactFullPath = Path.Combine(workDir, unsignedArtifactName);

                    await using (var unsignedApkStream = await unsignedApkStorage
                                     .GetFileAsync(unsignedArtifactData.UnsignedArtifactStorageKey, ct))
                    {
                        await using (var fileStream = File.OpenWrite(unsignedArtifactFullPath))
                        {
                            await unsignedApkStream.CopyToAsync(fileStream, ct);
                        }
                    }

                    return KeyValuePair.Create(unsignedArtifactFullPath, unsignedArtifactData);
                })))
                .ToImmutableDictionary();
            
            Directory.CreateDirectory(signingOutputDir);
            var signingResult = await apkSigner.SignApksAsync(task.AndroidApplicationId,
                downloadedUnsignedArtifacts.Keys, signingOutputDir, ct);
            
            var signedArtifactDatas = await Task.WhenAll(
                signingResult.Select(async signingResult =>
                {
                    var signedApkFileName = Path.GetFileName(signingResult.SignedApkFullPath);
                    var signedApkSha256 = await CommonMethods.GetFileSha256Async(signingResult.SignedApkFullPath, ct);
                    var signedApkSizeBytes = new FileInfo(signingResult.SignedApkFullPath).Length;
                    var signedApkStorageKey = $"{signedApkSha256}/{signedApkFileName}";

                    var idSigFileName = Path.GetFileName(signingResult.IdSigFileFullPath);
                    var idSigFileSha256 = await CommonMethods.GetFileSha256Async(signingResult.IdSigFileFullPath, ct);
                    var idSigSizeBytes = new FileInfo(signingResult.IdSigFileFullPath).Length;
                    var idSigFileStorageKey = $"{idSigFileSha256}/{idSigFileName}";

                    await Task.WhenAll(
                        SaveFileToSignedStorage(signedApkStorageKey, signingResult.SignedApkFullPath, ct),
                        SaveFileToSignedStorage(idSigFileStorageKey, signingResult.IdSigFileFullPath, ct));

                    var unsignedArtifactData = downloadedUnsignedArtifacts[signingResult.UnsignedApkFullPath];

                    return new SignedArtifactData
                    {
                        UnsignedArtifactId = unsignedArtifactData.UnsignedArtifactId,
                        SignedApkFileData = new StorageFileData
                        {
                            FileName = signedApkFileName,
                            FileSha256 = signedApkSha256,
                            FileSizeBytes = signedApkSizeBytes,
                            FileStorageKey = signedApkStorageKey
                        },
                        SignedApkIdSigFileData = new StorageFileData
                        {
                            FileName = idSigFileName,
                            FileSha256 = idSigFileSha256,
                            FileSizeBytes = idSigSizeBytes,
                            FileStorageKey = idSigFileStorageKey
                        }
                    };
                }));

            await eventPublisher.PublishEventAsync(new SigningSucceededEvent
            {
                BuildWorkflowId = task.BuildWorkflowId,
                SignedArtifactsData = signedArtifactDatas,
                PublishDateTimeOffset = DateTimeOffset.UtcNow
            }, ct);
        }
        catch (Exception e)
        {
            logger.LogError("Error during apk signing: {ErrorMessage}", e.Message);

            await eventPublisher.PublishEventAsync(new SigningFailedEvent
            {
                BuildWorkflowId = task.BuildWorkflowId,
                ErrorMessage = e.Message,
                PublishDateTimeOffset = DateTimeOffset.UtcNow
            }, ct);
        }
        finally
        {
            if (Directory.Exists(workDir))
                Directory.Delete(workDir, true);
        }
    }

    private async Task SaveFileToSignedStorage(string fileStorageKey, string fileFullPath, CancellationToken ct)
    {
        await using (var idSigFileStream = File.OpenRead(fileFullPath))
        {
            await signedApkStorage.SaveFileAsync(fileStorageKey, idSigFileStream, ct);
        }
    }
}