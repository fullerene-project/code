using Fullerene.Shared.Common.Extensions;
using Fullerene.Shared.Hosting.Extensions;
using Fullerene.Manager.Api.Settings;

namespace Fullerene.Manager.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureSettings<ProjectSettings>(configuration);

        return services.AddStartupTaskExecutor();
    }
}