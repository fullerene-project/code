using Fullerene.Shared.Common.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fullerene.Shared.Hosting.HostedServices;

public sealed class StartupTaskExecutor(
    IEnumerable<IStartupTask> tasks,
    ILogger<StartupTaskExecutor> logger) : IHostedLifecycleService
{
    public async Task StartingAsync(CancellationToken ct)
    {
        logger.LogInformation("Starting executing startup tasks");
        foreach (var task in tasks)
        {
            var taskName = task.GetType().Name;
            logger.LogInformation("Executing startup task {TaskName}", taskName);
            try
            {
                await task.ExecuteAsync(ct);
            }
            catch (Exception e)
            {
                logger.LogError("Error during executing startup task {TaskName}: {ErrorMessage}", taskName, e.Message);
                throw;
            }
        }
        logger.LogInformation("Startup tasks executing ended");
    }

    public Task StartAsync(CancellationToken ct) => Task.CompletedTask;

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

    public Task StartedAsync(CancellationToken ct) => Task.CompletedTask;

    public Task StoppingAsync(CancellationToken ct) => Task.CompletedTask;

    public Task StoppedAsync(CancellationToken ct) => Task.CompletedTask;
}