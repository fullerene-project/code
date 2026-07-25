using Fullerene.Manager.Domain.Models.WorkflowEvents;

namespace Fullerene.Manager.Domain.Models;

public sealed class BuildWorkflow
{
    public Guid Id { get; private set; }
    public Guid AndroidAppPackageVersionId { get; init; }

    public AndroidAppPackageVersion AndroidAppPackageVersion { get; private set; }
    public ICollection<Artifact> Artifacts { get; private set; }
    public ICollection<WorkflowEvent> WorkflowEvents { get; private set; }

    private BuildWorkflow(Guid id, Guid androidAppPackageVersionId)
    {
        Id = id;
        AndroidAppPackageVersionId = androidAppPackageVersionId;
    }

    public static BuildWorkflow CreateNew(Guid androidAppPackageVersionId)
    {
        return new BuildWorkflow(
            id: Guid.CreateVersion7(),
            androidAppPackageVersionId: androidAppPackageVersionId);
    }
}