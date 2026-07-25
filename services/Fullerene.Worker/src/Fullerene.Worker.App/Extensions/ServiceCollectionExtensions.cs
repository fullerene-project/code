using Fullerene.Shared.Hosting.Extensions;

namespace Fullerene.Worker.App.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddApp(
        this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddStartupTaskExecutor();
    }
}