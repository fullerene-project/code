using Fullerene.Shared.Contracts.Build;

namespace Fullerene.Worker.Application.Abstractions;

public interface IContainerNixBuilder
{
    Task<string> StartNixPackageBuildAsync(BuildTask buildTask, CancellationToken ct);
}