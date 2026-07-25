using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

public sealed class StandaloneSingleAbiArtifactDataRecord : ArtifactDataRecord
{
    public required CpuArchitecture CpuArchitecture { get; init; }
}