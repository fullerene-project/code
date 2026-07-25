using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

public sealed class AbiArtifactSplitDataRecord : ArtifactSplitDataRecord
{
    public required CpuArchitecture CpuArchitecture { get; init; }
}