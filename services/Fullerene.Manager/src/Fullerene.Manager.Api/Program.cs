using Fullerene.Shared.Common.Extensions;
using Fullerene.Manager.Api.Extensions;
using Fullerene.Manager.Api.Settings;
using Fullerene.Manager.Application.Cqrs.Commands;
using Fullerene.Manager.Application.Cqrs.Queries;
using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Application.Extensions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Infrastructure.Extensions;
using JasperFx.Resources;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Host.UseWolverine(options => options.AddFullereneManagerMessaging(configuration));

builder.Services.AddResourceSetupOnStartup();

builder.Services
    .AddOpenApi()
    .AddCors()
    .AddProblemDetails()
    .AddPresentation(configuration)
    .AddApplication(configuration)
    .AddInfrastructure(configuration);

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.MapOpenApi();
app.MapScalarApiReference();

var projectSettings = configuration.GetSettings<ProjectSettings>(nameof(ProjectSettings));

app.MapGet("/license", () =>
    Results.Ok(new
    {
        LicenseTextUrl = projectSettings.LicenseTextUrl,
        LicenseHtmlUrl = projectSettings.LicenseHtmlUrl
    }));

app.MapGet("/source-code", () =>
    Results.Ok(new
    {
        SourceCodeUrl = projectSettings.SourceCodeUrl
    }));


var v1Api = app.MapGroup("/v1");


var repositories = v1Api.MapGroup("/repositories");

repositories.MapGet("/", async (IMessageBus messageBus, [AsParameters] GetNixReposQuery query) =>
{
    var nixRepoDtos = await messageBus.InvokeAsync<IEnumerable<NixRepoDto>>(query);
    return Results.Ok(nixRepoDtos);
});

repositories.MapPost("/update", async (IMessageBus messageBus) =>
{
    await messageBus.InvokeAsync(new UpdateAllNixReposCommand());
    return Results.Ok();
});

repositories.MapPost("/{repoId:guid}/update", async (IMessageBus messageBus, Guid repoId) =>
{
    await messageBus.InvokeAsync(new UpdateNixReposCommand { NixRepoIds = [repoId] });
    return Results.Ok();
});

repositories.MapPost("/", async (IMessageBus messageBus, [FromBody] AddNixRepoCommand command) =>
{
    var repo = await messageBus.InvokeAsync<NixPackageRepo>(command);
    return Results.Ok(repo);
});


var apps = v1Api.MapGroup("/apps");

apps.MapGet("/", async (IMessageBus messageBus, [AsParameters] GetAndroidAppPackagesQuery query) =>
{
    var androidAppDtos = await messageBus.InvokeAsync<IEnumerable<AndroidAppPackageDto>>(query);
    return Results.Ok(androidAppDtos);
});

apps.MapPatch("/latest", async (IMessageBus messageBus, [FromBody] DownloadLatestSuitableAppVersionQuery query) =>
{
    var neededArtifacts = await messageBus.InvokeAsync<IEnumerable<SignedArtifactDownloadData>>(query);
    return Results.Ok(neededArtifacts);
});

apps.MapPost("/{appId:guid}/track", async (IMessageBus messageBus, Guid appId) =>
{
    await messageBus.InvokeAsync(new StartTrackingAndroidAppCommand { AndroidAppId = appId });
    return Results.Ok();
});


var artifacts = v1Api.MapGroup("/artifacts");

artifacts.MapGet("/", async (IMessageBus messageBus, [AsParameters] GetArtifactsQuery query) =>
{
    var artifactDtos = await messageBus.InvokeAsync<IEnumerable<ArtifactDto>>(query);

    return Results.Ok(artifactDtos);
});

artifacts.MapGet("/{artifactId:guid}/download", async (IMessageBus messageBus, Guid artifactId) =>
{
    var signedArtifactDownloadData = await messageBus.InvokeAsync<SignedArtifactDownloadData>(
        new DownloadArtifactQuery { ArtifactId = artifactId });

    return Results.Ok(signedArtifactDownloadData);
});

var versions = v1Api.MapGroup("/versions");

versions.MapGet("/", async (IMessageBus messageBus, [AsParameters] GetAndroidAppPackageVersionsQuery query) =>
{
    var versionDtos = await messageBus.InvokeAsync<IEnumerable<AndroidAppPackageVersionDto>>(query);

    return Results.Ok(versionDtos);
});

versions.MapPatch("/download", async (IMessageBus messageBus, [FromBody] DownloadVersionQuery query) =>
{
    var signedArtifactsDownloadData = await messageBus.InvokeAsync<IEnumerable<SignedArtifactDownloadData>>(query);

    return Results.Ok(signedArtifactsDownloadData);
});

app.Run();