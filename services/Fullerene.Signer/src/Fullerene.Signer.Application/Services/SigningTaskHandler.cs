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
        await eventPublisher.PublishEventAsync(new SigningStartedEvent { UnsignedArtifactId = task.UnsignedArtifactId }, ct);

        var workDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(workDir);

        var apkName = Path.GetFileName(task.UnsignedApkStorageKey);

        var unsignedApkFullPath = Path.Combine(workDir, apkName);

        var signingOutputDir = Path.Combine(workDir, "result");

        try
        {
            await using (var unsignedApkStream = await unsignedApkStorage.GetFileAsync(task.UnsignedApkStorageKey, ct))
            {
                await using (var fileStream = File.OpenWrite(unsignedApkFullPath))
                {
                    await unsignedApkStream.CopyToAsync(fileStream, ct);
                }
            }

            Directory.CreateDirectory(signingOutputDir);
            var signingResult = await apkSigner.SignApkAsync(task.AndroidAppId, unsignedApkFullPath, signingOutputDir, ct);

            var signedApkFileName = Path.GetFileName(signingResult.SignedApkFullPath);
            var signedApkSha256 = await CommonMethods.GetFileSha256Async(signingResult.SignedApkFullPath, ct);
            var signedApkSizeBytes = new FileInfo(signingResult.SignedApkFullPath).Length;
            var signedApkStorageKey = $"{signedApkSha256}/{signedApkFileName}";

            var idSigFileName = Path.GetFileName(signingResult.IdSigFileFullPath);
            var idSigFileSha256 = await CommonMethods.GetFileSha256Async(signingResult.IdSigFileFullPath, ct);
            var idSigSizeBytes = new FileInfo(signingResult.IdSigFileFullPath).Length;
            var idSigFileStorageKey = $"{idSigFileSha256}/{idSigFileName}";

            await using (var signedApkStream = File.OpenRead(signingResult.SignedApkFullPath))
            {
                await signedApkStorage.SaveFileAsync(signedApkStorageKey, signedApkStream, ct);
            }

            await using (var idSigFileStream = File.OpenRead(signingResult.IdSigFileFullPath))
            {
                await signedApkStorage.SaveFileAsync(idSigFileStorageKey, idSigFileStream, ct);
            }

            await eventPublisher.PublishEventAsync(new SigningSucceededEvent
            {
                UnsignedArtifactId = task.UnsignedArtifactId,
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
            }, ct);
        }
        catch (Exception e)
        {
            logger.LogError("Error during apk signing: {ErrorMessage}", e.Message);

            await eventPublisher.PublishEventAsync(new SigningFailedEvent
            {
                ArtifactId = task.UnsignedArtifactId,
                ErrorText = e.Message,
                PublishDateTimeOffset = DateTimeOffset.UtcNow
            }, ct);

            throw;
        }
        finally
        {
            if (Directory.Exists(workDir))
                Directory.Delete(workDir, true);
        }
    }
}