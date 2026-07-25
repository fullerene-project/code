namespace Fullerene.Manager.Domain.Models;

public abstract class ArtifactSplitDataRecord : ArtifactDataRecord
{
    public required string SplitId { get; init; }
    public required string ModuleName { get; init; }
}