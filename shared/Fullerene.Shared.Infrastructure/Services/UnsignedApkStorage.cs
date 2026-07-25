using Fullerene.Shared.Common.Abstractions.Storage;
using Fullerene.Shared.Infrastructure.Abstractions;
using Fullerene.Shared.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Fullerene.Shared.Infrastructure.Services;

public sealed class UnsignedApkStorage : IUnsignedApkStorage
{
    private readonly UnsignedApkS3BucketSettings _settings;
    private readonly IGeneralS3Storage _storage;

    public UnsignedApkStorage(IGeneralS3Storage storage,
        IOptions<UnsignedApkS3BucketSettings> settings)
    {
        _settings = settings.Value;
        _storage = storage;
    }

    public async Task SaveFileAsync(string destinationPath, Stream fileStream, CancellationToken ct)
    {
        await _storage.SaveFileAsync(_settings.BucketName, destinationPath, fileStream, ct);
    }

    public Task<Stream> GetFileAsync(string key, CancellationToken ct)
    {
        return _storage.GetFileAsync(_settings.BucketName, key, ct);
    }
}