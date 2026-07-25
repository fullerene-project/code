using Fullerene.Signer.App.Extensions;
using Fullerene.Signer.Application.Extensions;
using Fullerene.Signer.Infrastructure.Extensions;
using Wolverine;

var builder = Host.CreateApplicationBuilder(args);
var configuration = builder.Configuration;

builder.UseWolverine(options => options.AddFullereneSignerMessaging(configuration));

builder.Services
    .AddApp(configuration)
    .AddApplication(configuration)
    .AddInfrastructure(configuration);

var host = builder.Build();
host.Run();
