using Fullerene.Signer.Application.Dtos;

namespace Fullerene.Signer.Application.Abstractions;

public interface IApkSigner
{
    public Task<SigningResult> SignApkAsync(string androidAppId, string alignedApkFullPath,
        string outputDirectoryFullPath, CancellationToken ct);
}