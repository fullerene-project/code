using Fullerene.Shared.Common.Exceptions;
using Fullerene.Shared.Common.Extensions;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Shared.Contracts.Signing;
using Fullerene.Shared.Infrastructure.Extensions;
using Fullerene.Shared.Infrastructure.Settings;
using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Cqrs.Commands;
using Fullerene.Manager.Infrastructure.Persistence;
using Fullerene.Manager.Infrastructure.Services;
using Fullerene.Manager.Infrastructure.Settings;
using Fullerene.Manager.Infrastructure.StartupTasks;
using JasperFx;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.Postgresql;
using Wolverine.EntityFrameworkCore;
using Wolverine.RabbitMQ;

namespace Fullerene.Manager.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new AppConfigurationException("\"DefaultConnection\" configuration not found");

        services.ConfigureSettings<RabbitMqEventQueueSubscriptionSettings>(configuration);
        services.ConfigureSettings<RabbitMqTaskQueueMappingSettings>(configuration);

        return services
            .AddTransient<IGitCommitHistoryFetcher, GitCommitHistoryFetcher>()
            .AddTransient<IAndroidAppNixPackageMetaPuller, AndroidAppNixPackageMetaPuller>()
            .AddStartupTask<GitPresenceCheckTask>()
            .AddStartupTask<NixPresenceCheckTask>()
            .AddStartupTask<ApplyDbMigrationsTask>()
            .AddSingleton<INixFlakeUrlFormatter, NixFlakeUrlFormatter>()
            .AddFullereneStorage(configuration)
            .AddFullerenePersistence(connectionString);
    }

    private static IServiceCollection AddFullerenePersistence(
        this IServiceCollection services, string postgresConnectionString)
    {
        services.AddDbContextWithWolverineIntegration<AppDbContext>(options =>
        {
            options.UseNpgsql(postgresConnectionString);
        });

        services.AddScoped<IApplicationContext, AppDbContext>();

        return services;
    }

    public static WolverineOptions AddFullereneManagerMessaging(
        this WolverineOptions wolverineOptions, IConfiguration configuration)
    {
        var postgresConnectionString = configuration.GetConnectionString("DefaultConnection")
                                       ?? throw AppConfigurationException.ConfigNotFound("DefaultConnection");

        var taskQueueMappingSettings = configuration
            .GetSettings<RabbitMqTaskQueueMappingSettings>(nameof(RabbitMqTaskQueueMappingSettings));

        var eventsSubscriptionSettings = configuration
            .GetSettings<RabbitMqEventQueueSubscriptionSettings>(nameof(RabbitMqEventQueueSubscriptionSettings));

        wolverineOptions.Services.CritterStackDefaults(options =>
        {
            options.Production.ResourceAutoCreate = AutoCreate.CreateOrUpdate;
        });

        wolverineOptions.ListenToRabbitQueue(eventsSubscriptionSettings.QueueName)
            .ListenerCount(eventsSubscriptionSettings.ConcurrencyLimit);

        var buildTaskQueueName = taskQueueMappingSettings.QueueNameByTaskName.GetValueOrDefault(nameof(BuildTask))
                                 ?? throw new Exception($"No queue name found for {nameof(BuildTask)} task");
        wolverineOptions.PublishMessage<BuildTask>().ToRabbitQueue(buildTaskQueueName);

        var signingTaskQueueName = taskQueueMappingSettings.QueueNameByTaskName.GetValueOrDefault(nameof(SigningTask))
                                 ?? throw new Exception($"No queue name found for {nameof(SigningTask)} task");
        wolverineOptions.PublishMessage<SigningTask>().ToRabbitQueue(signingTaskQueueName);

        wolverineOptions.PersistMessagesWithPostgresql(postgresConnectionString);
        wolverineOptions.UseEntityFrameworkCoreTransactions();

        wolverineOptions.Policies.UseDurableInboxOnAllListeners();
        wolverineOptions.Policies.UseDurableOutboxOnAllSendingEndpoints();
        wolverineOptions.Discovery.IncludeAssembly(typeof(AddNixRepoCommand).Assembly);

        wolverineOptions.AddFullereneMessaging(configuration, transport =>
        {
            transport.BindExchange(nameof(BuildStartedEvent)).ToQueue(eventsSubscriptionSettings.QueueName);
            transport.BindExchange(nameof(BuildFailedEvent)).ToQueue(eventsSubscriptionSettings.QueueName);
            transport.BindExchange(nameof(BuildSucceededEvent)).ToQueue(eventsSubscriptionSettings.QueueName);
            transport.BindExchange(nameof(SigningStartedEvent)).ToQueue(eventsSubscriptionSettings.QueueName);
            transport.BindExchange(nameof(SigningFailedEvent)).ToQueue(eventsSubscriptionSettings.QueueName);
            transport.BindExchange(nameof(SigningSucceededEvent)).ToQueue(eventsSubscriptionSettings.QueueName);
        });

        return wolverineOptions;
    }
}