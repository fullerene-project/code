using Fullerene.Worker.App.Extensions;
using Fullerene.Worker.Application.Extensions;
using Fullerene.Worker.Infrastructure.Extensions;
using Wolverine;

var builder = Host.CreateApplicationBuilder(args);
var configuration = builder.Configuration;

builder.UseWolverine(options => options.AddFullereneWorkerMessaging(configuration));

builder.Services
    .AddApp(configuration)
    .AddApplication(configuration)
    .AddInfrastructure(configuration);

var host = builder.Build();
host.Run();
