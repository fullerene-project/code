using System.Text.Json;
using Fullerene.Shared.Common;
using Fullerene.Shared.Common.Abstractions.Messaging;
using Fullerene.Shared.Common.Abstractions.Storage;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Shared.Domain.Models;
using Fullerene.Worker.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Fullerene.Worker.Application.Services;

public sealed class BuildTaskHandler(
    IContainerNixBuilder containerNixBuilder,
    IUnsignedApkStorage unsignedApkStorage,
    IEventPublisher eventPublisher,
    ILogger<BuildTaskHandler> logger)
{
    private const string ManifestFileName = "manifest.json";
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task Handle(BuildTask buildTask, CancellationToken ct)
    {
        string? resultFolder = null;
        try
        {
            await eventPublisher.PublishEventAsync(new BuildStartedEvent { BuildWorkflowId = buildTask.BuildWorkflowId, }, ct);

            resultFolder = await containerNixBuilder.StartNixPackageBuildAsync(buildTask, ct);

            var manifestPath = Path.Combine(resultFolder, ManifestFileName);
            await using var manifestFileStream = File.OpenRead(manifestPath);
            var manifest = await JsonSerializer
                .DeserializeAsync<NixBuildOutputManifest>(manifestFileStream, JsonSerializerOptions, cancellationToken: ct);

            if (manifest is null)
            {
                logger.LogCritical("Build task for workflow \"{BuildWorkflowId}\" " +
                                   "did not produce \"{ManifestFileName}\" build manifest file",
                    buildTask.BuildWorkflowId, ManifestFileName);

                throw new Exception($"Build task for workflow \"{buildTask.BuildWorkflowId}\" " +
                                    $"did not produce \"{ManifestFileName}\" build manifest file)");
            }

            if (manifest.Entries.Count == 0)
            {
                logger.LogCritical("Build task for workflow \"{BuildWorkflowId}\" manifest contains no entries", buildTask.BuildWorkflowId);
                throw new Exception($"Build task for workflow \"{buildTask.BuildWorkflowId}\" manifest contains less than 1 entry");
            }

            var buildResultManifest = new BuildResultManifest
            {
                Entries = new List<BuildResultEntry>(manifest.Entries.Count),
                ReleaseChannel = (ReleaseChannel)manifest.ReleaseChannel
            };

            var filesToUpload = new HashSet<(string filePath, string storageKey)>(manifest.Entries.Count);

            foreach (var entry in manifest.Entries)
            {
                logger.LogInformation("Processing build result entry: \"{FileName}\"", entry.FileName);
                var unsignedApkFilePath = Path.Combine(resultFolder, entry.FileName);
                var unsignedApkSha256 = await CommonMethods.GetFileSha256Async(unsignedApkFilePath, ct);
                var unsignedApkSizeBytes = new FileInfo(unsignedApkFilePath).Length;

                if (!string.Equals(unsignedApkSha256, entry.FileSha256, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogCritical("Manifest entry sha256: \"{ManifestSha256}\" is different " +
                                       "from actual file sha256: \"{ActualSha256}\"",
                        entry.FileSha256, unsignedApkSha256);

                    throw new Exception($"Manifest entry sha256: \"{entry.FileSha256}\" is different " +
                                        $"from actual file sha256: \"{unsignedApkSha256}\"");
                }

                if (unsignedApkSizeBytes != entry.FileSizeBytes)
                {
                    logger.LogCritical("Manifest entry file size bytes: \"{ManifestFileSizeBytes}\" is different " +
                                       "from actual file size bytes: \"{ActualFileSizeBytes}\"",
                        entry.FileSizeBytes, unsignedApkSizeBytes);

                    throw new Exception($"Manifest entry file size bytes: \"{entry.FileSizeBytes}\" is different " +
                                        $"from actual file size bytes: \"{unsignedApkSizeBytes}\"");
                }

                var unsignedApkStorageKey = Path.Combine(unsignedApkSha256, entry.FileName);

                filesToUpload.Add((unsignedApkFilePath, unsignedApkStorageKey));

                buildResultManifest.Entries.Add(new BuildResultEntry
                {
                    UnsignedApkStorageKey = unsignedApkStorageKey,
                    FileName = entry.FileName,
                    FileSha256 = entry.FileSha256,
                    FileSizeBytes = entry.FileSizeBytes,
                    ArtifactType = (ArtifactType)entry.ArtifactType,
                    MinApiLevel = entry.MinApiLevel,
                    TargetApiLevel = entry.TargetApiLevel,
                    VersionCode = entry.VersionCode,
                    SplitId = entry.SplitId,
                    ModuleName = entry.ModuleName,
                    CpuArchitectures = entry.CpuArchitectures?.Select(x => (CpuArchitecture)x).ToHashSet(),
                    SingleCpuArchitecture = (CpuArchitecture?)entry.SingleCpuArchitecture,
                    DensityAlias = (ScreenDensityAlias?)entry.DensityAlias,
                    DensityDpi = entry.DensityDpi,
                    LanguageTargeting = entry.LanguageTargeting,
                    DeliveryType = (DeliveryType?)entry.DeliveryType,
                    AssetModuleType = (AssetModuleType?)entry.AssetModuleType,
                    TextureCompressionFormat = (TextureCompressionFormat?)entry.TextureCompressionFormat
                });
            }

            await Task.WhenAll(filesToUpload.Select(async x =>
            {
                await using var unsignedApkFileStream = new FileStream(
                    x.filePath,
                    new FileStreamOptions
                    {
                        Mode = FileMode.Open,
                        Access = FileAccess.Read,
                        Share = FileShare.Read,
                        Options = FileOptions.Asynchronous
                    });

                await unsignedApkStorage.SaveFileAsync(x.storageKey, unsignedApkFileStream, ct);

                logger.LogInformation("Successfully saved unsigned apk file with key: {FileStorageKey}", x.storageKey);
            }));

            await eventPublisher.PublishEventAsync(new BuildSucceededEvent
            {
                BuildWorkflowId = buildTask.BuildWorkflowId,
                Manifest = buildResultManifest
            }, ct);
        }
        catch (Exception e)
        {
            await eventPublisher.PublishEventAsync(new BuildFailedEvent
            {
                BuildWorkflowId = buildTask.BuildWorkflowId,
                ErrorText = e.Message
            }, ct);
            throw;
        }
        finally
        {
            if (resultFolder is not null && Directory.Exists(resultFolder))
            {
                Directory.Delete(resultFolder, true);
            }
        }
    }
}