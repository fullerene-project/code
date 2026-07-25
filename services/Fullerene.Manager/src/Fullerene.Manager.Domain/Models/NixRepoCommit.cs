namespace Fullerene.Manager.Domain.Models;

public sealed class NixRepoCommit
{
    public Guid Id { get; private set; }
    public Guid NixRepoId { get; private set; }

    public string CommitHash { get; private set; }
    public bool Processed { get; set; }
    public DateTimeOffset CommitDateTimeOffset { get; private set; }

    public NixPackageRepo NixRepo { get; set; }
    public ICollection<AndroidAppPackageVersion> AndroidAppPackageVersions { get; set; }

    private NixRepoCommit(
        Guid id,
        Guid nixRepoId,
        string commitHash,
        bool processed,
        DateTimeOffset commitDateTimeOffset)
    {
        Id = id;
        NixRepoId = nixRepoId;
        CommitHash = commitHash;
        Processed = processed;
        CommitDateTimeOffset = commitDateTimeOffset;
    }

    public static NixRepoCommit CreateNew(
        Guid nixRepoId,
        string commitHash,
        bool processed,
        DateTimeOffset commitDateTime)
    {
        return new NixRepoCommit(
            id: Guid.CreateVersion7(),
            nixRepoId: nixRepoId,
            commitHash: commitHash,
            processed: processed,
            commitDateTimeOffset: commitDateTime);
    }
}