using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Services;
using Fullerene.Manager.Application.Services.ArtifactMappers;
using Fullerene.Manager.Application.Settings;
using Fullerene.Shared.Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fullerene.Manager.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureSettings<BuildStrategySettings>(configuration);

        return services
            .AddSingleton<IBuildResultToArtifactMapperService, BuildResultToArtifactMapperService>()
            .AddSingleton<IBuildResultToArtifactMapper, BuildResultToAbiArtifactSplitMapper>()
            .AddSingleton<IBuildResultToArtifactMapper, BuildResultToAssetsArtifactSplitMapper>()
            .AddSingleton<IBuildResultToArtifactMapper, BuildResultToBaseArtifactSplitMapper>()
            .AddSingleton<IBuildResultToArtifactMapper, BuildResultToDensityArtifactSplitMapper>()
            .AddSingleton<IBuildResultToArtifactMapper, BuildResultToFeatureArtifactSplitMapper>()
            .AddSingleton<IBuildResultToArtifactMapper, BuildResultToLanguageArtifactSplitMapper>()
            .AddSingleton<IBuildResultToArtifactMapper, BuildResultToStandaloneSingleAbiArtifactMapper>()
            .AddSingleton<IBuildResultToArtifactMapper, BuildResultToStandaloneUniversalArtifactMapper>();
    }
}