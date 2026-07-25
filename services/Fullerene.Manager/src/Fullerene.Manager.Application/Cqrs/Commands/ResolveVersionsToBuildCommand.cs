using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Application.Extensions;
using Fullerene.Manager.Application.Settings;
using Fullerene.Manager.Application.Util;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.WorkflowEvents;
using Fullerene.Shared.Common.Abstractions.Messaging;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Shared.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fullerene.Manager.Application.Cqrs.Commands;

public sealed class ResolveVersionsToBuildCommand
{
    public required IEnumerable<CombinedNixAppPackageIdentifier>? PackageIdentifiers { get; init; } = null;
}

public sealed class ResolveVersionsToBuildCommandHandler(
    IApplicationContext context,
    INixFlakeUrlFormatter nixFlakeUrlFormatter,
    ITaskPublisher taskPublisher,
    IOptions<BuildStrategySettings> buildStrategySettings)
{
    public async Task Handle(ResolveVersionsToBuildCommand command, CancellationToken ct)
    {
        var settings = buildStrategySettings.Value;

        var versionsToBuild = new HashSet<ToBuildVersionData>();

        var requestedPackageIdentifiers = command.PackageIdentifiers?.ToHashSet();

        var repoIds = requestedPackageIdentifiers?
            .Select(x => x.NixPackageRepoId)
            .ToArray();
        var androidApplicationIds = requestedPackageIdentifiers?
            .Select(x => x.AndroidApplicationId)
            .ToArray();
        var packageNames = requestedPackageIdentifiers?
            .Select(x => x.NixPackageName)
            .ToArray();

        var versionsByPackages = await context.AndroidAppPackages
            .AsNoTracking()
            .WhereIf(
                requestedPackageIdentifiers is not null,
                x =>
                    repoIds.Contains(x.NixPackageRepoId) &&
                    androidApplicationIds.Contains(x.AndroidApplicationId) &&
                    packageNames.Contains(x.NixPackageName))
            .WhereIf(
                settings.BuildStrategy == BuildStrategy.OnSubscription,
                x => x.IsTracked)
            .Select(x => x.AndroidAppPackageVersions
                .OrderByDescending(y => y.BaseVersionCode)
                .ThenByDescending(y => y.AppVersionReleaseDate)
                .Select(y => new ToBuildVersionData
                {
                    VersionId = y.Id,
                    RepoId = y.NixRepoCommit.NixRepoId,
                    ReleaseChannel = y.ReleaseChannel,
                    HasSuccessfulBuild = y.BuildWorkflows
                        .Any(w => w.WorkflowEvents
                            .Any(e => e.EventType == WorkflowEventType.BuildSucceeded)),
                    GitCommitHash = y.NixRepoCommit.CommitHash,
                    GitRepoUrl = y.NixRepoCommit.NixRepo.GitRepositoryUrl,
                    PackageName = y.NixPackageName,
                    AndroidApplicationId = y.AndroidApplicationId,
                }))
            .ToArrayAsync(ct);

        if (requestedPackageIdentifiers is not null)
        {
            versionsByPackages = versionsByPackages
                .Select(x => x
                    .Where(y => requestedPackageIdentifiers.Contains(
                        new CombinedNixAppPackageIdentifier(y.RepoId, y.AndroidApplicationId, y.PackageName))))
                .ToArray();
        }

        foreach (var versions in versionsByPackages)
        {
            var stables = settings.BuildLatestFromChannels[ReleaseChannel.Stable];
            var betas = settings.BuildLatestFromChannels[ReleaseChannel.Beta];
            var alphas = settings.BuildLatestFromChannels[ReleaseChannel.Alpha];

            foreach (var version in versions)
            {
                var shouldBuildThisVersion = false;

                if (stables < 1 && betas < 1 && alphas < 1)
                {
                    break;
                }
                if (version.ReleaseChannel == ReleaseChannel.Stable && stables > 0)
                {
                    stables--;
                    betas = 0;
                    alphas = 0;
                    shouldBuildThisVersion = true;
                }
                if (version.ReleaseChannel == ReleaseChannel.Beta && betas > 0)
                {
                    betas--;
                    alphas = 0;
                    shouldBuildThisVersion = true;
                }
                if (version.ReleaseChannel == ReleaseChannel.Alpha && alphas > 0)
                {
                    alphas--;
                    shouldBuildThisVersion = true;
                }

                if (shouldBuildThisVersion && !version.HasSuccessfulBuild)
                {
                    versionsToBuild.Add(version);
                }
            }
        }

        foreach (var versionToBuild in versionsToBuild)
        {
            var buildWorkflow = BuildWorkflow.CreateNew(versionToBuild.VersionId);

            context.BuildWorkflows.Add(buildWorkflow);

            var nixFlakeUrl = nixFlakeUrlFormatter.FormatNixFlakeUrl(versionToBuild.GitRepoUrl, versionToBuild.GitCommitHash);

            var buildTask = new BuildTask
            {
                BuildWorkflowId = buildWorkflow.Id,
                NixFlakeUrl = nixFlakeUrl,
                PackageName = versionToBuild.PackageName,
                PublishDateTimeOffset = DateTimeOffset.UtcNow,
            };

            await taskPublisher.PublishTaskAsync(buildTask, ct);
        }

        await context.SaveChangesAsync(ct);
    }

    private sealed class ToBuildVersionData
    {
        public required Guid VersionId { get; set; }
        public required Guid RepoId { get; set; }
        public required ReleaseChannel ReleaseChannel { get; set; }
        public required bool HasSuccessfulBuild { get; set; }
        public required string GitCommitHash { get; set; }
        public required string GitRepoUrl { get; set; }
        public required string PackageName { get; set; }
        public required string AndroidApplicationId { get; set; }
    }
}