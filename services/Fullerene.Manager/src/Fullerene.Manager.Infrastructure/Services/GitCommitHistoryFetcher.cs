using CliWrap;
using CliWrap.Buffered;
using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Dtos;
using Fullerene.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Fullerene.Manager.Infrastructure.Services;

public sealed class GitCommitHistoryFetcher(
    ILogger<GitCommitHistoryFetcher> logger) : IGitCommitHistoryFetcher
{
    public async Task<IEnumerable<GitCommitFetchedData>> GetCommits(string gitRepoUrl, CancellationToken ct)
    {
        var tempRepoFolderPath =
            Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var commitHistory = new List<GitCommitFetchedData>();

        try
        {
            await Cli.Wrap("git")
                .WithArguments(args => args
                    .Add("-c").Add("core.askpass=true")
                    .Add("clone")
                    .Add("--bare")
                    .Add("--filter=tree:0")
                    .Add(gitRepoUrl)
                    .Add(tempRepoFolderPath))
                .WithEnvironmentVariables(env => env
                    .Set("GIT_TERMINAL_PROMPT", "0")
                    .Set("GIT_LFS_SKIP_SMUDGE", "1"))
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line => logger.LogInformation(line)))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(line => logger.LogError(line)))
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .ExecuteBufferedAsync(ct);

            await Cli.Wrap("git")
                .WithArguments(args => args
                    .Add("--no-pager")
                    .Add($"--git-dir={tempRepoFolderPath}")
                    .Add("log")
                    .Add("--reverse")
                    .Add("--format=%H|%cI"))
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line =>
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2 &&
                        DateTimeOffset.TryParse(parts[1], out var commitDateTimeOffset))
                    {
                        commitHistory.Add(new GitCommitFetchedData
                        {
                            Hash = parts[0],
                            CommitDateTimeOffset = commitDateTimeOffset,
                        });
                        return;
                    }
                    throw new InternalException($"Invalid git commit output data format: \"{gitRepoUrl}\"");
                }))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(line => logger.LogError(line)))
                .ExecuteAsync(ct);
        }
        catch (CliWrap.Exceptions.CommandExecutionException e)
        {
            logger.LogError("Error during git execution: {Error}", e.Message);
            throw;
        }
        finally
        {
            if (Directory.Exists(tempRepoFolderPath))
                Directory.Delete(tempRepoFolderPath, true);
        }

        return commitHistory;
    }
}