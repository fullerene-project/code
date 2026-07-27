using CliWrap;
using Fullerene.Worker.Application.Abstractions;
using Fullerene.Worker.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fullerene.Worker.Infrastructure.Services;

/// <summary>
/// builds nix packages by running nixos podman containers
/// </summary>
public sealed class PodmanNixBuilder : INixBuilder
{
    private readonly PodmanNixBuilderSettings _podmanNixBuilderSettings;
    private readonly ILogger<PodmanNixBuilder> _logger;

    private const string NixFlakeUrlVarName = "NIX_FLAKE_URL";
    private const string PackageNameEnvVarName = "PACKAGE_NAME";
    private const string OutputDirectoryEnvVarName = "OUTPUT_DIRECTORY";
    
    private const string BuildCommand = 
        $"nix --extra-experimental-features 'nix-command flakes' --option build-users-group \"\" " +
        $"build \"${NixFlakeUrlVarName}#\\\"${PackageNameEnvVarName}\\\"\" &&" +
        $"rm -rf \"${OutputDirectoryEnvVarName}\" &&" +
        $"mkdir -p \"${OutputDirectoryEnvVarName}\" &&" +
        $"cp -L result/*.apk \"${OutputDirectoryEnvVarName}\"/. &&" +
        $"cp -L result/manifest.json \"${OutputDirectoryEnvVarName}\"/. &&" +
        $"ls \"${OutputDirectoryEnvVarName}\"";
    
    public PodmanNixBuilder(
        IOptions<PodmanNixBuilderSettings> podmanSettings,
        ILogger<PodmanNixBuilder> logger)
    {
        _podmanNixBuilderSettings = podmanSettings.Value;
        _logger = logger;
    }

    public async Task StartNixPackageBuildAsync(string nixFlakeUrl,
        string packageName, string resultDir, CancellationToken ct)
    {
        if (!Directory.Exists(resultDir))
            Directory.CreateDirectory(resultDir);
        
        var tempNixContainerName = Guid.NewGuid().ToString();
        var tempNixContainerOutputDirectory = $"/{Guid.NewGuid().ToString()}";
        
        _logger.LogInformation("Starting nix package build in podman container, flake URL: \"{FlakeUrl}\", package name: \"{PackageName}\"",
            nixFlakeUrl, packageName);

        try
        {
            await Cli.Wrap("podman")
                .WithArguments(args => args
                    .Add("--remote")
                    .Add("run")
                    .Add("--name").Add(tempNixContainerName)
                    .Add("-e").Add($"{NixFlakeUrlVarName}={nixFlakeUrl}")
                    .Add("-e").Add($"{PackageNameEnvVarName}={packageName}")
                    .Add("-e").Add($"{OutputDirectoryEnvVarName}={tempNixContainerOutputDirectory}")
                    .Add("-v").Add($"{_podmanNixBuilderSettings.NixVolume}:/nix:z")
                    .Add(_podmanNixBuilderSettings.NixImage)
                    .Add("sh").Add("-c").Add(BuildCommand))
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line =>
                    _logger.LogInformation("[PODMAN-STDOUT] {Line}", line)))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(line =>
                    _logger.LogInformation("[PODMAN-STDERR] {Line}", line)))
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .ExecuteAsync(ct);

            await Cli.Wrap("podman")
                .WithArguments(args => args
                    .Add("--remote")
                    .Add("cp").Add($"{tempNixContainerName}:{tempNixContainerOutputDirectory}/.").Add(resultDir))
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line =>
                    _logger.LogInformation("[PODMAN-STDOUT] {Line}", line)))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(line =>
                    _logger.LogInformation("[PODMAN-STDERR] {Line}", line)))
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .ExecuteAsync(ct);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "Error during nix package building, flake URL: \"{FlakeUrl}\", package name: \"{PackageName}\"",
                nixFlakeUrl, packageName);
            throw;
        }
        finally
        {
            try
            {
                using var fiveSecCtSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                
                await Cli.Wrap("podman")
                    .WithArguments(args => args
                        .Add("--remote")
                        .Add("container")
                        .Add("rm").Add("--force").Add(tempNixContainerName))
                    .WithStandardOutputPipe(PipeTarget.ToDelegate(line =>
                        _logger.LogInformation("[PODMAN-STDOUT] {Line}", line)))
                    .WithStandardErrorPipe(PipeTarget.ToDelegate(line =>
                        _logger.LogInformation("[PODMAN-STDERR] {Line}", line)))
                    .WithValidation(CommandResultValidation.None)
                    .ExecuteAsync(fiveSecCtSource.Token);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Temp nix build container removing failed. Container name: \"{ContainerName}\", error message: \"{ErrorMessage}\"",
                    tempNixContainerName, e.Message);
            }
        }

        _logger.LogInformation("Nix package build finished, task: flake URL: \"{FlakeUrl}\", package name: \"{PackageName}\"",
            nixFlakeUrl, packageName);
    }
}