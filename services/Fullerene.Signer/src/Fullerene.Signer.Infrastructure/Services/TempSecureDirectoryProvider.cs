using Fullerene.Signer.Application.Abstractions;
using Fullerene.Signer.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Fullerene.Signer.Infrastructure.Services;

public sealed class TempSecureDirectoryProvider : ITempSecureDirectoryProvider
{
    private readonly TempSecureDirectorySettings _settings;
    private readonly HashSet<string> _createdDirectories = new();

    public TempSecureDirectoryProvider(IOptions<TempSecureDirectorySettings> settings)
    {
        _settings = settings.Value;
    }

    public string GetTempSecureDirectory()
    {
        var newTempDir = Path.Combine(_settings.Path, Guid.NewGuid().ToString());
        Directory.CreateDirectory(newTempDir);
        _createdDirectories.Add(newTempDir);
        return newTempDir;
    }

    public void Dispose()
    {
        foreach (var createdDirectory in _createdDirectories)
        {
            if (Directory.Exists(createdDirectory))
                Directory.Delete(createdDirectory, true);
        }
    }
}