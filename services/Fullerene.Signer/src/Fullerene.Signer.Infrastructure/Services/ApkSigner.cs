using System.Security.Cryptography.X509Certificates;
using CliWrap;
using Fullerene.Signer.Application.Abstractions;
using Fullerene.Signer.Application.Dtos;
using Fullerene.Signer.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;

namespace Fullerene.Signer.Infrastructure.Services;

public sealed class ApkSigner(
    IApkSigningCertificateGenerator certificateGenerator,
    ITempSecureDirectoryProvider tempSecureDirectoryProvider,
    ILogger<ApkSigner> logger) : IApkSigner
{
    private const string KeyPassEnvVarName = "KEY_PASS";

    public async Task<IEnumerable<SigningResult>> SignApksAsync(
        string androidApplicationId,
        IEnumerable<string> unsignedApksFullPaths,
        string outputDirectoryFullPath,
        CancellationToken ct)
    {
        var workDir = tempSecureDirectoryProvider.GetTempSecureDirectory();
        var keyFileName = $"{androidApplicationId}-{Guid.NewGuid()}.p12";
        var keyFullPath = Path.Combine(workDir, keyFileName);
        
        try
        {
            using var signingCertificate = certificateGenerator.CreateCertificate(androidApplicationId);
            var keyPassword = Guid.NewGuid().ToString();
            var p12Bytes = signingCertificate.Export(X509ContentType.Pkcs12, keyPassword);
            await File.WriteAllBytesAsync(keyFullPath, p12Bytes, ct);

            var signingResults = await
                Task.WhenAll(unsignedApksFullPaths.Select(async unsignedApkFullPath =>
            {
                var apkFileName = Path.GetFileName(unsignedApkFullPath);
                var idSigFileName = apkFileName + ".idsig";
                var signedApkFullPath = Path.Combine(outputDirectoryFullPath, apkFileName);
                var idSigFileFullPath = Path.Combine(outputDirectoryFullPath, idSigFileName);

                await Cli.Wrap("apksigner")
                    .WithArguments(args => args
                        .Add("sign")
                        .Add("--ks").Add(keyFullPath)
                        .Add("--ks-pass").Add($"env:{KeyPassEnvVarName}")
                        .Add("--v2-signing-enabled").Add("true")
                        .Add("--v3-signing-enabled").Add("false")
                        .Add("--v4-signing-enabled").Add("true")
                        .Add("--min-sdk-version").Add(18)
                        .Add("--out").Add(signedApkFullPath)
                        .Add(unsignedApkFullPath))
                    .WithEnvironmentVariables(configure =>
                        configure.Set(KeyPassEnvVarName, keyPassword))
                    .WithStandardOutputPipe(PipeTarget.ToDelegate(line =>
                        logger.LogInformation("[APK-SIGNER-OUT] {Line}", line)))
                    .WithStandardErrorPipe(PipeTarget.ToDelegate(line =>
                        logger.LogError("[APK-SIGNER-ERR] {Line}", line)))
                    .WithValidation(CommandResultValidation.ZeroExitCode)
                    .ExecuteAsync(ct);

                if (!File.Exists(signedApkFullPath))
                    throw new FileNotFoundException("signed apk file not found after signing", signedApkFullPath);

                if (!File.Exists(idSigFileFullPath))
                    throw new FileNotFoundException("idsig file not found after signing", idSigFileFullPath);

                return new SigningResult
                {
                    UnsignedApkFullPath = unsignedApkFullPath,
                    SignedApkFullPath = signedApkFullPath,
                    IdSigFileFullPath = idSigFileFullPath,
                };
            }));

            return signingResults;
        }
        catch (Exception e)
        {
            logger.LogError("Error during apk signing: {ErrorMessage}", e.Message);
            throw;
        }
    }
}