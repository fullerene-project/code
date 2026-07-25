namespace Fullerene.Shared.Common.Abstractions.Storage;

public interface IFileStorage
{
    Task SaveFileAsync(string destinationPath, Stream fileStream, CancellationToken ct);

    Task<Stream> GetFileAsync(string key, CancellationToken ct);
}