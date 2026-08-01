using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Application.Extensions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Shared.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Fullerene.Manager.Application.Cqrs.Queries;

public sealed class DownloadLatestSuitableAppVersionQuery
{
    public required Guid AppId { get; init; }
    public required ClientDeviceInfo? ClientDeviceInfo { get; init; }
    public required ReleaseChannel[] ReleaseChannels { get; init; }
    public required int? CurrentBaseVersionCode { get; init; }
    public required bool StandaloneApkOnly { get; init; }
}

public sealed class DownloadLatestSuitableAppVersionQueryHandler(
    IApplicationContext context,
    IMessageBus messageBus,
    ILogger<DownloadLatestSuitableAppVersionQueryHandler> logger)
{
    public async Task<IEnumerable<SignedArtifactDownloadData>> Handle(
        DownloadLatestSuitableAppVersionQuery query,
        CancellationToken ct)
    {
        var appExists = await context.AndroidAppPackages
            .AnyAsync(x => x.Id == query.AppId, ct);

        if (!appExists)
            throw new Exception($"No app found with id \"{query.AppId}\"");
        
        var latestVersions = await context.AndroidAppPackageVersions
            .Where(ver =>
                ver.AndroidAppPackage.Id == query.AppId &&
                query.ReleaseChannels.Contains(ver.ReleaseChannel) &&
                ver.BuildWorkflows.Any(bw => bw.Status == BuildWorkflowStatus.SigningSucceeded))
            .WhereIf(query.CurrentBaseVersionCode is not null, ver =>
                ver.BaseVersionCode > query.CurrentBaseVersionCode)
            .OrderByDescending(ver => ver.BaseVersionCode)
            .ThenByDescending(ver => ver.AppVersionReleaseDate)
            .Select(ver => ver.Id)
            .ToArrayAsync(ct);

        for (int i = 0; i < latestVersions.Length; i++)
        {
            var currentVersionId = latestVersions[i];
            try
            {
                var downloadData = await messageBus.InvokeAsync<IEnumerable<SignedArtifactDownloadData>>(
                    new DownloadVersionQuery 
                    { 
                        VersionId = currentVersionId,
                        StandaloneApkOnly = query.StandaloneApkOnly,
                        ClientDeviceInfo = query.ClientDeviceInfo
                    }, ct);
                
                return downloadData;
            }
            catch (Exception e)
            {
                logger.LogWarning("Error during version selection. Version id: \"{VersionId}\" Error message: {Message}", currentVersionId, e.Message);
            }
        }
        
        throw new Exception("No suitable app versions found");
    }
}