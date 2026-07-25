namespace Fullerene.Manager.Domain.Models;

public sealed class NixPackageRepo
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }
    public string GitRepositoryUrl { get; private set; }

    public ICollection<NixRepoCommit> NixRepoCommits { get; set; }
    public ICollection<AndroidAppPackage> AndroidApps { get; set; }

    private NixPackageRepo(Guid id, string name,
        string gitRepositoryUrl)
    {
        Id = id;
        Name = name;
        GitRepositoryUrl = gitRepositoryUrl;
    }

    public static NixPackageRepo CreateNew(string name, string gitRepositoryUrl)
    {
        return new NixPackageRepo(
            id: Guid.CreateVersion7(),
            name: name,
            gitRepositoryUrl: gitRepositoryUrl);
    }
}