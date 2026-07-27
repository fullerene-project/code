using Fullerene.Shared.Common.Extensions;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Shared.Infrastructure.Extensions;
using Fullerene.Worker.Application.Abstractions;
using Fullerene.Worker.Application.Services;
using Fullerene.Worker.Infrastructure.Services;
using Fullerene.Worker.Infrastructure.Settings;
using Fullerene.Worker.Infrastructure.StartupTasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Fullerene.Worker.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureSettings<PodmanNixBuilderSettings>(configuration);

        services.ConfigureSettings<RabbitMqBuildTaskSubscriptionSettings>(configuration);

        return services
            .AddStartupTask<PodmanPresenceCheckTask>()
            .AddTransient<INixBuilder, PodmanNixBuilder>()
            .AddFullereneStorage(configuration);
    }

    public static WolverineOptions AddFullereneWorkerMessaging(
        this WolverineOptions wolverineOptions, IConfiguration configuration)
    {
        var queueSettings = configuration
            .GetSettings<RabbitMqBuildTaskSubscriptionSettings>(nameof(RabbitMqBuildTaskSubscriptionSettings));

        wolverineOptions.ListenToRabbitQueue(queueSettings.QueueName)
            .ListenerCount(queueSettings.ConcurrencyLimit);

        wolverineOptions.PublishMessage<BuildStartedEvent>().ToRabbitExchange(nameof(BuildStartedEvent));
        wolverineOptions.PublishMessage<BuildFailedEvent>().ToRabbitExchange(nameof(BuildFailedEvent));
        wolverineOptions.PublishMessage<BuildSucceededEvent>().ToRabbitExchange(nameof(BuildSucceededEvent));

        wolverineOptions.Discovery.IncludeAssembly(typeof(BuildTaskHandler).Assembly);

        wolverineOptions.DefaultExecutionTimeout = TimeSpan.FromHours(1);

        wolverineOptions.AddFullereneMessaging(configuration, _ => { });

        return wolverineOptions;
    }
}