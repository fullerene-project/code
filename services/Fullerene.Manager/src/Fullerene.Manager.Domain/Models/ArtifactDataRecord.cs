using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Domain.Models;

public abstract class ArtifactDataRecord
{
    public required Guid Id { get; init; }

    public required ArtifactType ArtifactType { get; init; }
    public required int VersionCode { get; init; }
    public required int MinApiLevel { get; init; }
    public required int TargetApiLevel { get; init; }

    public ICollection<Artifact> Artifacts { get; private set; }
}