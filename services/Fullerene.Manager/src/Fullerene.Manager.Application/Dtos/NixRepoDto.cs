namespace Fullerene.Manager.Application.Dtos;

public sealed class NixRepoDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string GitRepositoryUrl { get; init; }
}