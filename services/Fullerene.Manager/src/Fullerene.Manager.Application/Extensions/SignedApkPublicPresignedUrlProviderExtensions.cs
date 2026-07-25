using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Domain.Models;
using Fullerene.Shared.Common.Abstractions.Storage;
using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Extensions;

public static class SignedApkPublicPresignedUrlProviderExtensions
{
    public static SignedArtifactDownloadData GetDownloadData(this ISignedApkPublicPresignedUrlProvider provider,
        StorageFileData fileData, StorageFileData? idSigFileData)
    {
        var signedApkPublicDownloadUrl =
            provider.GetPublicTempPresignedDownloadUrl(
                fileData.FileStorageKey, 300);

        var signedApkIdSigPublicDownloadUrl = idSigFileData is not null
            ? provider.GetPublicTempPresignedDownloadUrl(
                idSigFileData.FileStorageKey, 300)
            : null;

        return new SignedArtifactDownloadData
        {
            ApkFileData = new DownloadableFileData
            {
                DownloadUrl = signedApkPublicDownloadUrl,
                FileName = fileData.FileName,
                FileSha256 = fileData.FileSha256,
                FileSizeBytes = fileData.FileSizeBytes
            },
            ApkIdSigFileData = signedApkIdSigPublicDownloadUrl is not null
                ? new DownloadableFileData
                {
                    DownloadUrl = signedApkIdSigPublicDownloadUrl,
                    FileName = idSigFileData.FileName,
                    FileSha256 = idSigFileData.FileSha256,
                    FileSizeBytes = idSigFileData.FileSizeBytes
                }
                : null
        };
    }

    public static SignedArtifactDownloadData GetDownloadData(
        this ISignedApkPublicPresignedUrlProvider provider, ArtifactDto artifactDto)
    {
        return provider.GetDownloadData(artifactDto.FileData, artifactDto.IdSigFileData);
    }

    public static SignedArtifactDownloadData GetDownloadData(
        this ISignedApkPublicPresignedUrlProvider provider, Artifact artifact)
    {
        return provider.GetDownloadData(artifact.FileData, artifact.IdSigFileData);
    }
}