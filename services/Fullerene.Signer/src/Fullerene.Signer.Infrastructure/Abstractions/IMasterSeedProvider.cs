namespace Fullerene.Signer.Infrastructure.Abstractions;

public interface IMasterSeedProvider
{
    byte[] GetMasterSeed();
}