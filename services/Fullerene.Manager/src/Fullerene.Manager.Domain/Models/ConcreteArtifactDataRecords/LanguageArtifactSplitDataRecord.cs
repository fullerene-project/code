namespace Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;

public sealed class LanguageArtifactSplitDataRecord : ArtifactSplitDataRecord
{
    public required string LanguageTargeting { get; init; }
}