using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Fullerene.Manager.Application.Cqrs.Commands;

public sealed class UpdateNixReposCommand
{
    public required IEnumerable<Guid> NixRepoIds { get; init; }
}

public sealed class UpdateNixReposCommandHandler(
    IApplicationContext context,
    IGitCommitHistoryFetcher gitCommitHistoryFetcher,
    IAndroidAppNixPackageMetaPuller androidAppNixPackageMetaPuller,
    IMessageBus messageBus,
    INixFlakeUrlFormatter nixFlakeUrlFormatter,
    ILogger<UpdateNixReposCommandHandler> logger)
{
    public async Task Handle(UpdateNixReposCommand command, CancellationToken ct)
    {
        var repos = await context.NixPackageRepos
            .Where(repo => command.NixRepoIds.Contains(repo.Id))
            .Include(repo => repo.NixRepoCommits
                .Where(com => com.Processed)
                .OrderByDescending(com => com.CommitDateTimeOffset)
                .Take(1))
            .Select(repo => new
            {
                Repo = repo,
                LastProcessedCommit = repo.NixRepoCommits.FirstOrDefault()
            })
            .ToArrayAsync(ct);

        logger.LogInformation("Updating nix repos: [\"{RepoNames}\"]", string.Join("\", \"", repos.Select(x => x.Repo.Name)));

        foreach (var repo in repos)
        {
            logger.LogInformation("Processing repo: \"{RepoName}\"", repo.Repo.Name);
            var commits = (await gitCommitHistoryFetcher
                .GetCommits(repo.Repo.GitRepositoryUrl, ct))
                .OrderBy(commit => commit.CommitDateTimeOffset)
                .ToList();

            var unprocessedCommitsData =
                (repo.LastProcessedCommit is not null && commits.Select(commit => commit.Hash).Contains(repo.LastProcessedCommit.CommitHash))
                ? commits
                    .SkipWhile(x => x.Hash != repo.LastProcessedCommit.CommitHash)
                    .Skip(1)
                    .ToList()
                : commits;

            if (unprocessedCommitsData.Count == 0) continue;
            logger.LogInformation("Unprocessed commit hashes: [\"{CommitHashes}]\"", string.Join("\", \"", unprocessedCommitsData.Select(c => c.Hash)));

            var commitEntitiesByHash = await context.NixRepoCommits
                .Where(x => commits.Select(commit => commit.Hash).Contains(x.CommitHash))
                .ToDictionaryAsync(x => x.CommitHash, x => x, ct);

            var processedDerivationHashes = new HashSet<string>();

            var pendingVersionHashes = new HashSet<string>();
            var pendingPackages = new HashSet<CombinedNixAppPackageIdentifier>();
            foreach (var unprocessedCommitData in unprocessedCommitsData)
            {
                logger.LogInformation("Processing commit: \"{CommitHash}\"", unprocessedCommitData.Hash);

                var commit = commitEntitiesByHash.GetValueOrDefault(unprocessedCommitData.Hash);

                if (commit is null)
                {
                    commit = NixRepoCommit.CreateNew(
                        nixRepoId: repo.Repo.Id,
                        commitHash: unprocessedCommitData.Hash,
                        processed: false,
                        commitDateTime: unprocessedCommitData.CommitDateTimeOffset.ToUniversalTime());

                    context.NixRepoCommits.Add(commit);
                }

                if (commit.Processed) continue;

                var nixFlakeUrl = nixFlakeUrlFormatter
                    .FormatNixFlakeUrl(repo.Repo.GitRepositoryUrl, unprocessedCommitData.Hash);

                var packages =
                    await androidAppNixPackageMetaPuller.GetNixPackageMeta(nixFlakeUrl, ct);

                if (packages is null) continue;

                var packagesByHashes = packages
                    .Where(x => !processedDerivationHashes.Contains(x.DerivationHash))
                    .ToDictionary(x => x.DerivationHash, x => x);

                foreach (var packageByHash in packagesByHashes)
                {
                    processedDerivationHashes.Add(packageByHash.Key);
                }

                var existingVersionHashes = await context.AndroidAppPackageVersions
                    .Where(x => packagesByHashes.Keys.Contains(x.NixDerivationHash))
                    .Select(x => x.NixDerivationHash)
                    .ToArrayAsync(ct);

                var newPackageVersionMetas = packagesByHashes.Values.ToHashSet();
                newPackageVersionMetas.RemoveWhere(meta =>
                    existingVersionHashes.Contains(meta.DerivationHash) ||
                    pendingVersionHashes.Contains(meta.DerivationHash));

                foreach (var newPackageVersionMeta in newPackageVersionMetas)
                    pendingVersionHashes.Add(newPackageVersionMeta.DerivationHash);

                var newAppPackageVersions = newPackageVersionMetas
                    .Select(x =>
                    {
                        return new AndroidAppPackageVersion
                        {
                            Id = Guid.CreateVersion7(),
                            NixDerivationHash = x.DerivationHash,
                            AppVersionString = x.AppVersionString,
                            ReleaseNotes = x.ReleaseNotes,
                            NixPackageRepoId = repo.Repo.Id,
                            CommitHash = commit.CommitHash,
                            NixPackageRevision = x.NixPackageRevision,
                            ReleaseChannel = x.ReleaseChannel,
                            AppVersionReleaseDate = x.AppReleaseDate,
                            AppLogoUrl = x.AppLogoUrl,
                            AppName = x.AppName,
                            AppSummary = x.AppSummary,
                            AppDescription = x.AppDescription,
                            AppLicense = x.AppLicense,
                            NixPackageName = x.PackageName,
                            AndroidApplicationId = x.AndroidApplicationId,
                            BaseVersionCode = x.BaseVersionCode
                        };
                    });

                context.AndroidAppPackageVersions.AddRange(newAppPackageVersions);

                var newPackageCombinedIds = newAppPackageVersions
                    .Select(x => new CombinedNixAppPackageIdentifier(repo.Repo.Id, x.AndroidApplicationId, x.NixPackageName))
                    .ToHashSet();

                var existingPackages = await context.AndroidAppPackages
                    .Where(x => x.NixPackageRepoId == repo.Repo.Id)
                    .Select(x => new CombinedNixAppPackageIdentifier(x.NixPackageRepoId, x.AndroidApplicationId, x.NixPackageName))
                    .ToArrayAsync(ct);

                var notExistingPackages = newPackageCombinedIds
                    .Except(existingPackages)
                    .Except(pendingPackages)
                    .ToArray();

                foreach (var notExistingPackage in notExistingPackages)
                    pendingPackages.Add(notExistingPackage);

                context.AndroidAppPackages.AddRange(notExistingPackages
                    .Select(x => AndroidAppPackage
                        .CreateNew(x.NixPackageRepoId, x.NixPackageName, x.AndroidApplicationId, false)));

                commit.Processed = true;

                await messageBus.PublishAsync(new ResolveVersionsToBuildCommand { PackageIdentifiers = newPackageCombinedIds });
            }
        }

        await context.SaveChangesAsync(ct);
    }
}