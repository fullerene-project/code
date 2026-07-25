namespace Fullerene.Manager.Application.Dtos;

public sealed class GitCommitFetchedData
{
    public required string Hash { get; set; }
    public DateTimeOffset CommitDateTimeOffset { get; set; }
}