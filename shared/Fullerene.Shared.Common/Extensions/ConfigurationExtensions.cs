using Fullerene.Shared.Common.Abstractions;
using Fullerene.Shared.Common.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Fullerene.Shared.Common.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets settings from configuration, validates it and return
    /// </summary>
    /// <param name="configuration">Your configuration provider</param>
    /// <param name="sectionName">Configuration section name</param>
    /// <typeparam name="TSettings">Settings type</typeparam>
    /// <returns></returns>
    /// <exception cref="AppConfigurationException">Configuration not found or failed validation</exception>
    public static TSettings GetSettings<TSettings>(this IConfiguration configuration, string sectionName)
        where TSettings : ISelfValidatingConfiguration
    {
        var settings = configuration
                           .GetSection(sectionName)
                           .Get<TSettings>()
                       ?? throw AppConfigurationException.ConfigNotFound(sectionName);

        settings.ValidateOrThrow();

        return settings;
    }
}