using System.Security.Cryptography;
using System.Text;
using Fullerene.Signer.Application.Abstractions;
using Fullerene.Signer.Infrastructure.Abstractions;

namespace Fullerene.Signer.Infrastructure.Services;

public sealed class PerAppPrivateKeyDeriver(
    IMasterSeedProvider masterSeedProvider) : IPerAppPrivateKeyDeriver
{
    public byte[] DerivePrivateKey(string appId)
    {
        var masterSeed = masterSeedProvider.GetMasterSeed();
        var appIdBytes = Encoding.UTF8.GetBytes(appId);

        var privateKeyBytes = HKDF.DeriveKey(
            HashAlgorithmName.SHA256,
            masterSeed,
            outputLength: 32,
            salt: null,
            appIdBytes);

        return privateKeyBytes;
    }
}