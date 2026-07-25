namespace Fullerene.Shared.Common.Abstractions.Storage;

public interface ISignedApkPublicPresignedUrlProvider
{
    string GetPublicTempPresignedDownloadUrl(string key, int ttlSeconds);
}