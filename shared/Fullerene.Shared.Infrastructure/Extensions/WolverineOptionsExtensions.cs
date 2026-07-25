using Fullerene.Shared.Common.Abstractions.Messaging;
using Fullerene.Shared.Common.Extensions;
using Fullerene.Shared.Infrastructure.Services;
using Fullerene.Shared.Infrastructure.Settings;
using JasperFx.CodeGeneration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace Fullerene.Shared.Infrastructure.Extensions;

public static class WolverineOptionsExtensions
{
    public static WolverineOptions AddFullereneMessaging(
        this WolverineOptions wolverineOptions, IConfiguration configuration,
        Action<RabbitMqTransportExpression> configureTransport)
    {
        var rabbitMqConnectionSettings = configuration
            .GetSettings<RabbitMqConnectionSettings>(nameof(RabbitMqConnectionSettings));

        var transport = wolverineOptions.UseRabbitMq(factory =>
        {
            factory.HostName = rabbitMqConnectionSettings.Host;
            factory.Port = rabbitMqConnectionSettings.Port;
            factory.UserName = rabbitMqConnectionSettings.User;
            factory.Password = rabbitMqConnectionSettings.Password;
        }).AutoProvision();

        wolverineOptions.CodeGeneration.TypeLoadMode = TypeLoadMode.Dynamic;

        wolverineOptions.Services.AddScoped<ITaskPublisher, RabbitMqTaskPublisher>();
        wolverineOptions.Services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();

        configureTransport(transport);

        return wolverineOptions;
    }
}