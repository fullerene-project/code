namespace Fullerene.Shared.Contracts.Build;

public sealed class BuildTask : FullereneMessage
{
    public required Guid BuildWorkflowId { get; init; }
    public required string NixFlakeUrl { get; init; }
    public required string PackageName { get; init; }
}