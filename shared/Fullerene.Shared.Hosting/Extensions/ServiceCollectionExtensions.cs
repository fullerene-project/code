using Fullerene.Shared.Hosting.HostedServices;
using Microsoft.Extensions.DependencyInjection;

namespace Fullerene.Shared.Hosting.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartupTaskExecutor(this IServiceCollection services)
    {
        return services.AddHostedService<StartupTaskExecutor>();
    }
}