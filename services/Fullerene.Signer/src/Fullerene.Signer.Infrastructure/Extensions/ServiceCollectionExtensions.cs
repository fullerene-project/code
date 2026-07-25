using Fullerene.Shared.Common.Extensions;
using Fullerene.Shared.Contracts.Signing;
using Fullerene.Shared.Infrastructure.Extensions;
using Fullerene.Signer.Application.Abstractions;
using Fullerene.Signer.Application.Services;
using Fullerene.Signer.Infrastructure.Abstractions;
using Fullerene.Signer.Infrastructure.Services;
using Fullerene.Signer.Infrastructure.Settings;
using Fullerene.Signer.Infrastructure.StartupTasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Fullerene.Signer.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureSettings<SignerIdentitySettings>(configuration);
        services.ConfigureSettings<SigningSettings>(configuration);

        services.ConfigureSettings<RabbitMqSigningTaskSubscriptionSettings>(configuration);
        services.ConfigureSettings<TempSecureDirectorySettings>(configuration);

        return services
            .AddSingleton<IECDsaDeriver, ECDsaDeriver>()
            .AddSingleton<IPerAppPrivateKeyDeriver, PerAppPrivateKeyDeriver>()
            .AddSingleton<IApkSigningCertificateGenerator, ApkSigningCertificateGenerator>()
            .AddStartupTask<ApkSignerPresenceCheckTask>()
            .AddScoped<IApkSigner, ApkSigner>()
            .AddTransient<IMasterSeedProvider, MasterSeedProvider>()
            .AddTransient<ITempSecureDirectoryProvider, TempSecureDirectoryProvider>()
            .AddFullereneStorage(configuration);
    }

    public static WolverineOptions AddFullereneSignerMessaging(
        this WolverineOptions wolverineOptions, IConfiguration configuration)
    {
        var queueSettings = configuration
            .GetSettings<RabbitMqSigningTaskSubscriptionSettings>(nameof(RabbitMqSigningTaskSubscriptionSettings));

        wolverineOptions.ListenToRabbitQueue(queueSettings.QueueName)
            .ListenerCount(queueSettings.ConcurrencyLimit);

        wolverineOptions.PublishMessage<SigningStartedEvent>().ToRabbitExchange(nameof(SigningStartedEvent));
        wolverineOptions.PublishMessage<SigningFailedEvent>().ToRabbitExchange(nameof(SigningFailedEvent));
        wolverineOptions.PublishMessage<SigningSucceededEvent>().ToRabbitExchange(nameof(SigningSucceededEvent));

        wolverineOptions.Discovery.IncludeAssembly(typeof(SigningTaskHandler).Assembly);

        wolverineOptions.AddFullereneMessaging(configuration, _ => { });

        return wolverineOptions;
    }
}