namespace Fullerene.Manager.Application.Exceptions;

public sealed class GitHistoryFetchException(string message, string? gitRepositoryUrl)
    : Exception($"Error during fetching git commits data: {message}")
{
    public string? GitRepositoryUrl { get; init; } = gitRepositoryUrl;
}