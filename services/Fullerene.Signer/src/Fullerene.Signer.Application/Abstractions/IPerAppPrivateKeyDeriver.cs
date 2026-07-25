namespace Fullerene.Signer.Application.Abstractions;

public interface IPerAppPrivateKeyDeriver
{
    byte[] DerivePrivateKey(string appId);
}