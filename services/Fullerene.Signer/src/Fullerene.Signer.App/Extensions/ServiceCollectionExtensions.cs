using Fullerene.Shared.Hosting.Extensions;

namespace Fullerene.Signer.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApp(
        this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddStartupTaskExecutor();
    }
}