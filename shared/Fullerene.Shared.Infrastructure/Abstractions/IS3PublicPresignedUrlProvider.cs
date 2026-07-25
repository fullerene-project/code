using Genbox.SimpleS3.Core.Abstracts.Clients;

namespace Fullerene.Shared.Infrastructure.Abstractions;

public interface IS3PublicPresignedUrlProvider
{
    string GetPublicTempPresignedDownloadUrl(string bucketName, string key, int ttlSeconds);
}