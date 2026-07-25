using Fullerene.Shared.Common.Abstractions.Storage;
using Fullerene.Shared.Infrastructure.Abstractions;
using Fullerene.Shared.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Fullerene.Shared.Infrastructure.Services;

public class SignedApkPublicPresignedUrlProvider : ISignedApkPublicPresignedUrlProvider
{
    private readonly SignedApkS3BucketSettings _settings;
    private readonly IS3PublicPresignedUrlProvider _publicUrlPresigner;

    public SignedApkPublicPresignedUrlProvider(
        IS3PublicPresignedUrlProvider publicUrlPresigner,
        IOptions<SignedApkS3BucketSettings> settings)
    {
        _settings = settings.Value;
        _publicUrlPresigner = publicUrlPresigner;
    }

    public string GetPublicTempPresignedDownloadUrl(string key, int ttlSeconds)
    {
        return _publicUrlPresigner.GetPublicTempPresignedDownloadUrl(_settings.BucketName, key, ttlSeconds);
    }
}