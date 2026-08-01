using Fullerene.Signer.Application.Dtos;

namespace Fullerene.Signer.Application.Abstractions;

/// <summary>
/// Signs apks). Deterministically generates certificates
/// based on the master key and Android application id.
/// </summary>
public interface IApkSigner
{
    public Task<IEnumerable<SigningResult>> SignApksAsync(
        string androidApplicationId,
        IEnumerable<string> unsignedApksFullPaths,
        string outputDirectoryFullPath,
        CancellationToken ct);
}