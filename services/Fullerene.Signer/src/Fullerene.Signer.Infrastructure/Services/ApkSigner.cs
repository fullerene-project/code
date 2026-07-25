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

    public async Task<SigningResult> SignApkAsync(string androidAppId,
        string alignedApkFullPath, string outputDirectoryFullPath, CancellationToken ct)
    {
        var workDir = tempSecureDirectoryProvider.GetTempSecureDirectory();

        var keyFileName = $"{androidAppId}-{Guid.NewGuid()}.p12";
        var keyFullPath = Path.Combine(workDir, keyFileName);
        var apkFileName = Path.GetFileName(alignedApkFullPath);
        var idSigFileName = apkFileName + ".idsig";
        var signedApkFullPath = Path.Combine(outputDirectoryFullPath, apkFileName);
        var idSigFileFullPath = Path.Combine(outputDirectoryFullPath, idSigFileName);

        try
        {
            using var signingCertificate = certificateGenerator.CreateCertificate(androidAppId);
            var keyPassword = Guid.NewGuid().ToString();
            var p12Bytes = signingCertificate.Export(X509ContentType.Pkcs12, keyPassword);
            await File.WriteAllBytesAsync(keyFullPath, p12Bytes, ct);

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
                    .Add(alignedApkFullPath))
                .WithEnvironmentVariables(configure =>
                    configure.Set(KeyPassEnvVarName, keyPassword))
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line =>
                    logger.LogInformation("[APK-SIGNER-OUT] {Line}", line)))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(line =>
                    logger.LogError("[APK-SIGNER-ERR] {Line}", line)))
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .ExecuteAsync(ct);

            if (!File.Exists(idSigFileFullPath))
                throw new FileNotFoundException("idsig file not found after signing", idSigFileFullPath);
        }
        catch (Exception e)
        {
            logger.LogError("Error during apk signing: {ErrorMessage}", e.Message);
            throw;
        }

        return new SigningResult
        {
            SignedApkFullPath = signedApkFullPath,
            IdSigFileFullPath = idSigFileFullPath
        };
    }
}