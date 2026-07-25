namespace Fullerene.Shared.Common.Abstractions;

public interface IStartupTask
{
    public Task ExecuteAsync(CancellationToken ct);
}