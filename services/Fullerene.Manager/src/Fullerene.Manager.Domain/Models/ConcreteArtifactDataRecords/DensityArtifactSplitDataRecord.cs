namespace Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

public sealed class DensityArtifactSplitDataRecord : ArtifactSplitDataRecord
{
    public required ScreenDensity Density { get; init; }
}