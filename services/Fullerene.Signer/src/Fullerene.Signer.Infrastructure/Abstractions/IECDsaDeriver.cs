using System.Security.Cryptography;

namespace Fullerene.Signer.Infrastructure.Abstractions;

public interface IECDsaDeriver
{
    ECDsa DeriveFromPrivateKey(byte[] privateKey);
}