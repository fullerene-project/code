using Fullerene.Shared.Infrastructure.Abstractions;
using Fullerene.Shared.Infrastructure.Util;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.DependencyInjection;

namespace Fullerene.Shared.Infrastructure.Services;

public sealed class GaragePublicPresignedUrlS3Provider(
    [FromKeyedServices(S3ClientRole.PublicUrlPresigner)] ISignedClient publicUrlPresigner) : IS3PublicPresignedUrlProvider
{
    public string GetPublicTempPresignedDownloadUrl(string bucketName, string key, int ttlSeconds)
    {
        var request = new GetObjectRequest(bucketName, key);
        return publicUrlPresigner.SignRequest(request, TimeSpan.FromSeconds(ttlSeconds));
    }
}