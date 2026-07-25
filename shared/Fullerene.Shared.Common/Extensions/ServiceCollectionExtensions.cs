using Fullerene.Shared.Common.Abstractions;
using Fullerene.Shared.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fullerene.Shared.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static TSettings ConfigureAndGetSettings<TSettings>(
        this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
        where TSettings : class, ISelfValidatingConfiguration
    {
        sectionName ??= typeof(TSettings).Name;
        var section = configuration.GetSection(sectionName);

        services.ConfigureAndValidateSettings<TSettings>(section);

        var settings = section.Get<TSettings>()
                       ?? throw AppConfigurationException.ConfigNotFound(sectionName);

        settings.ValidateOrThrow();

        return settings;
    }

    public static IServiceCollection ConfigureSettings<TSettings>(
        this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
        where TSettings : class, ISelfValidatingConfiguration
    {
        sectionName ??= typeof(TSettings).Name;
        var section = configuration.GetSection(sectionName);

        services.ConfigureAndValidateSettings<TSettings>(section);

        return services;
    }

    private static IServiceCollection ConfigureAndValidateSettings<TSettings>(
        this IServiceCollection services, IConfigurationSection section)
        where TSettings : class, ISelfValidatingConfiguration
    {
        services
            .AddOptions<TSettings>()
            .Bind(section)
            .Validate(settings =>
            {
                settings.ValidateOrThrow();
                return true;
            })
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddStartupTask<TStartupTask>(this IServiceCollection services)
        where TStartupTask : class, IStartupTask
    {
        return services.AddTransient<IStartupTask, TStartupTask>();
    }
}