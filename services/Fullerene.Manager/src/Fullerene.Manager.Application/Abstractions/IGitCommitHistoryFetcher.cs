using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Application.Exceptions;

namespace Fullerene.Manager.Application.Abstractions;

public interface IGitCommitHistoryFetcher
{
    /// <summary>
    /// Returns all git repository commits data 
    /// </summary>
    /// <param name="gitRepoUrl">Git repository URL</param>
    /// <exception cref="GitHistoryFetchException">Error during fetching commits</exception>
    /// <returns></returns>
    Task<IEnumerable<GitCommitFetchedData>> GetCommits(string gitRepoUrl, CancellationToken ct);
}