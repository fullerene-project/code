using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

public sealed class StandaloneUniversalArtifactDataRecord : ArtifactDataRecord
{
    public required ICollection<CpuArchitecture> CpuArchitectures { get; init; }
}