using Fullerene.Manager.Infrastructure.Persistence;
using Fullerene.Shared.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fullerene.Manager.Infrastructure.StartupTasks;

public sealed class ApplyDbMigrationsTask(IServiceScopeFactory scopeFactory) : IStartupTask
{
    public async Task ExecuteAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync(ct);
    }
}