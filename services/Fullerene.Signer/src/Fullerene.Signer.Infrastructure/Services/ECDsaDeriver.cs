using System.Security.Cryptography;
using Fullerene.Signer.Infrastructure.Abstractions;

namespace Fullerene.Signer.Infrastructure.Services;

public sealed class ECDsaDeriver : IECDsaDeriver
{
    public ECDsa DeriveFromPrivateKey(byte[] privateKey)
    {
        if (privateKey.Length != 32)
            throw new Exception("Private key length must be exactly 32 bytes");

        var ecParams = new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            D = privateKey
        };

        return ECDsa.Create(ecParams);
    }
}