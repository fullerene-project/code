using CliWrap;
using Fullerene.Shared.Contracts.Build;
using Fullerene.Worker.Application.Abstractions;
using Fullerene.Worker.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fullerene.Worker.Infrastructure.Services;

public sealed class PodmanNixBuilder : IContainerNixBuilder
{
    private readonly PodmanSettings _podmanSettings;
    private readonly ILogger<PodmanNixBuilder> _logger;

    private const string NixFlakeUrlVarName = "NIX_FLAKE_URL";
    private const string PackageNameEnvVarName = "PACKAGE_NAME";
    private const string OutFolderEnvVarName = "OUT_FOLDER";

    public PodmanNixBuilder(
        IOptions<PodmanSettings> podmanSettings,
        ILogger<PodmanNixBuilder> logger)
    {
        _podmanSettings = podmanSettings.Value;
        _logger = logger;
    }

    public async Task<string> StartNixPackageBuildAsync(BuildTask buildTask, CancellationToken ct)
    {
        var outFolder = Path.Combine("/out", buildTask.BuildWorkflowId.ToString());

        var buildCommand =
            $"nix --extra-experimental-features 'nix-command flakes' --option build-users-group \"\" " +
            $"build \"${NixFlakeUrlVarName}#\\\"${PackageNameEnvVarName}\\\"\" &&" +
            $"rm -rf \"${OutFolderEnvVarName}\" &&" +
            $"mkdir -p \"${OutFolderEnvVarName}\" &&" +
            $"cp -L result/*.apk \"${OutFolderEnvVarName}\"/. &&" +
            $"cp -L result/manifest.json \"${OutFolderEnvVarName}\"/. &&" +
            $"ls \"${OutFolderEnvVarName}\"";

        _logger.LogInformation("Starting nix package build, task: {TaskId}", buildTask.BuildWorkflowId);

        try
        {
            var result = await Cli.Wrap("podman")
                .WithArguments(args => args
                    .Add("--remote")
                    .Add("run")
                    .Add("-e").Add($"{NixFlakeUrlVarName}={buildTask.NixFlakeUrl}")
                    .Add("-e").Add($"{PackageNameEnvVarName}={buildTask.PackageName}")
                    .Add("-e").Add($"{OutFolderEnvVarName}={outFolder}")
                    .Add("--rm")
                    .Add("-v").Add($"{_podmanSettings.NixVolume}:/nix:z")
                    .Add("-v").Add($"{_podmanSettings.FileTransferVolume}:/out:z")
                    .Add(_podmanSettings.NixImage)
                    .Add("sh").Add("-c").Add(buildCommand))
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line =>
                    _logger.LogInformation("[PODMAN-OUT] {Line}", line)))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(line =>
                    _logger.LogInformation("[PODMAN-ERR] {Line}", line)))
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .ExecuteAsync(ct);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during nix package building, task: {TaskId}", buildTask.BuildWorkflowId);
            throw;
        }

        _logger.LogInformation("Nix package build finished, task: {TaskId}. out folder path: {OutPath}", buildTask.BuildWorkflowId, outFolder);

        return outFolder;
    }
}