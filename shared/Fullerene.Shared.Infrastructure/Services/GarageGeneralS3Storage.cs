using Fullerene.Shared.Domain.Exceptions;
using Fullerene.Shared.Infrastructure.Abstractions;
using Fullerene.Shared.Infrastructure.Util;
using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Fullerene.Shared.Infrastructure.Services;

public sealed class GarageGeneralS3Storage(
    [FromKeyedServices(S3ClientRole.General)] ISimpleClient s3Client) : IGeneralS3Storage
{
    public async Task SaveFileAsync(string bucketName,
        string key, Stream fileStream, CancellationToken ct)
    {
        var upload = s3Client.CreateUpload(bucketName, key);

        var result = await upload.UploadAsync(fileStream, ct);

        if (!result.IsSuccess)
            throw new InternalException("Upload failed");
    }

    public async Task<Stream> GetFileAsync(string bucketName, string key, CancellationToken ct)
    {
        var result = await s3Client.GetObjectAsync(bucketName, key, token: ct);

        if (!result.IsSuccess)
            throw new InternalException("Download failed");

        return result.Content;
    }
}