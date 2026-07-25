using System.Collections.Concurrent;
using System.Text.Json;
using CliWrap;
using CliWrap.Buffered;
using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Fullerene.Manager.Infrastructure.Services;

public sealed class AndroidAppNixPackageMetaPuller(
    ILogger<AndroidAppNixPackageMetaPuller> logger) : IAndroidAppNixPackageMetaPuller
{
    private static readonly string NixExpression = $@"
        pkgs: builtins.attrValues (builtins.mapAttrs (name: pkg: {{
        {nameof(AndroidAppNixPackageMeta.PackageName)} = name;
        {nameof(AndroidAppNixPackageMeta.DerivationHash)} = pkg.drvPath;
        {nameof(AndroidAppNixPackageMeta.AndroidApplicationId)} = pkg.passthru.androidApplicationId or null;
        {nameof(AndroidAppNixPackageMeta.AppLogoUrl)} = pkg.passthru.logoUrl or null;
        {nameof(AndroidAppNixPackageMeta.BaseVersionCode)} = pkg.passthru.baseVersionCode or null;
        {nameof(AndroidAppNixPackageMeta.AppVersionString)} = pkg.passthru.appVersionString or null;
        {nameof(AndroidAppNixPackageMeta.ReleaseChannel)} = pkg.passthru.releaseChannel or null;
        {nameof(AndroidAppNixPackageMeta.AppReleaseDate)} = pkg.passthru.appReleaseDate or null;
        {nameof(AndroidAppNixPackageMeta.ReleaseNotes)} = pkg.passthru.releaseNotes or null;
        {nameof(AndroidAppNixPackageMeta.NixPackageRevision)} = pkg.passthru.nixPackageRevision or null;
        {nameof(AndroidAppNixPackageMeta.AppName)} = pkg.passthru.appName or null;
        {nameof(AndroidAppNixPackageMeta.AppDescription)} = pkg.passthru.appDescription or null;
        {nameof(AndroidAppNixPackageMeta.AppSummary)} = pkg.passthru.appSummary or null;
        {nameof(AndroidAppNixPackageMeta.AppLicense)} = pkg.passthru.appLicense or null;
        }}) pkgs)";

    public async Task<IEnumerable<AndroidAppNixPackageMeta>?>
        GetNixPackageMeta(string nixFlakeUrl, CancellationToken ct)
    {
        var x8664LinuxFlakeUrl = $"{nixFlakeUrl}#packages.x86_64-linux";

        try
        {
            var result = await Cli.Wrap("nix")
                .WithArguments(args => args
                    .Add("--extra-experimental-features").Add("nix-command flakes")
                    .Add("eval")
                    .Add("--json")
                    .Add(x8664LinuxFlakeUrl)
                    .Add("--apply").Add(NixExpression))
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .ExecuteBufferedAsync(ct);

            using var jsonDocument = JsonDocument.Parse(result.StandardOutput);

            if (jsonDocument.RootElement.ValueKind != JsonValueKind.Array)
                throw new Exception("Nix commit json data isn't an array");

            var packageMetas = new List<AndroidAppNixPackageMeta>(jsonDocument.RootElement.GetArrayLength());

            foreach (var package in jsonDocument.RootElement.EnumerateArray())
            {
                var currentPackageName = "unknown";
                try
                {
                    currentPackageName = package.GetProperty(nameof(AndroidAppNixPackageMeta.PackageName)).GetString() ?? currentPackageName;

                    var meta = JsonSerializer.Deserialize<AndroidAppNixPackageMeta>(package);

                    if (meta is null)
                    {
                        throw new Exception("Deserialized package meta is null");
                    }

                    packageMetas.Add(meta);
                }
                catch (Exception e)
                {
                    logger.LogError("Error during parsing nix package meta. package name: \"{PackageName}\", " +
                                    "flake url: \"{FlakeUrl}\", error message: \"{ErrorMessage}\", raw JSON: \"{RawJson}\"",
                        currentPackageName, x8664LinuxFlakeUrl, e.Message, package.GetRawText());
                }
            }

            return packageMetas;
        }
        catch (Exception e)
        {
            logger.LogError("Error during pulling nix package meta: {ErrorMessage}", e.Message);

            return null;
        }
    }
}