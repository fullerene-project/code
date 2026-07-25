namespace Fullerene.Signer.Application.Abstractions;

public interface ITempSecureDirectoryProvider : IDisposable
{
    string GetTempSecureDirectory();
}