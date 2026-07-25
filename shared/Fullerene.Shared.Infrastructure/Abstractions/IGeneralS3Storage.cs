namespace Fullerene.Shared.Infrastructure.Abstractions;

public interface IGeneralS3Storage
{
    public Task SaveFileAsync(string bucketName,
        string key, Stream fileStream, CancellationToken ct);

    public Task<Stream> GetFileAsync(string bucketName,
        string key, CancellationToken ct);
}