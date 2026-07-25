namespace Fullerene.Manager.Domain.Models;

public sealed class AndroidAppPackage
{
    public Guid Id { get; private set; }
    public Guid NixPackageRepoId { get; private set; }
    public string NixPackageName { get; private set; }
    public string AndroidApplicationId { get; private set; }

    public bool IsTracked { get; set; }

    public NixPackageRepo NixRepo { get; set; }
    public ICollection<AndroidAppPackageVersion> AndroidAppPackageVersions { get; set; }

    private AndroidAppPackage() { }

    private AndroidAppPackage(
        Guid id,
        Guid nixPackageRepoId,
        string nixPackageName,
        string androidApplicationId,
        bool isTracked)
    {
        Id = id;
        NixPackageRepoId = nixPackageRepoId;
        NixPackageName = nixPackageName;
        AndroidApplicationId = androidApplicationId;
        IsTracked = isTracked;
    }

    public static AndroidAppPackage CreateNew(
        Guid nixPackageRepoId,
        string nixPackageName,
        string androidApplicationId,
        bool isTracked)
    {
        return new AndroidAppPackage(
            Guid.CreateVersion7(),
            nixPackageRepoId,
            nixPackageName,
            androidApplicationId,
            isTracked);
    }
}