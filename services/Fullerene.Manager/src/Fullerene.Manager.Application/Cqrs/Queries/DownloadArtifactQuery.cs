using Fullerene.Shared.Common.Abstractions.Storage;
using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Dtos;
using Fullerene.Shared.Domain.Exceptions;
using Fullerene.Shared.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fullerene.Manager.Application.Cqrs.Queries;

public sealed class DownloadArtifactQuery
{
    public required Guid ArtifactId { get; init; }
}

public sealed class DownloadArtifactQueryHandler(
    IApplicationContext context,
    ISignedApkPublicPresignedUrlProvider? signedApkPublicPresignedUrlProvider = null)
{
    public async Task<SignedArtifactDownloadData> Handle(DownloadArtifactQuery query, CancellationToken ct)
    {
        if (signedApkPublicPresignedUrlProvider is null)
            throw new InternalException("Public presigned url provider is not configured");

        var signedArtifactInfo = await context.Artifacts
            .AsNoTracking()
            .Where(x => x.Id == query.ArtifactId)
            .Select(x => new
            {
                x.FileData,
                x.IdSigFileData
            })
            .FirstOrDefaultAsync(ct);

        if (signedArtifactInfo is null)
            throw new NotFoundException($"Could not find artifact with id: \"{query.ArtifactId}\"");

        var signedApkPublicDownloadUrl =
            signedApkPublicPresignedUrlProvider.GetPublicTempPresignedDownloadUrl(
                signedArtifactInfo.FileData.FileStorageKey, 300);

        var signedApkIdSigPublicDownloadUrl = signedArtifactInfo.IdSigFileData is not null
            ? signedApkPublicPresignedUrlProvider.GetPublicTempPresignedDownloadUrl(
                signedArtifactInfo.IdSigFileData.FileStorageKey, 300)
            : null;

        return new SignedArtifactDownloadData
        {
            ApkFileData = new DownloadableFileData
            {
                DownloadUrl = signedApkPublicDownloadUrl,
                FileName = signedArtifactInfo.FileData.FileName,
                FileSha256 = signedArtifactInfo.FileData.FileSha256,
                FileSizeBytes = signedArtifactInfo.FileData.FileSizeBytes
            },
            ApkIdSigFileData = signedApkIdSigPublicDownloadUrl is not null
                ? new DownloadableFileData
                {
                    DownloadUrl = signedApkIdSigPublicDownloadUrl,
                    FileName = signedArtifactInfo.IdSigFileData.FileName,
                    FileSha256 = signedArtifactInfo.IdSigFileData.FileSha256,
                    FileSizeBytes = signedArtifactInfo.IdSigFileData.FileSizeBytes
                }
                : null
        };
    }
}