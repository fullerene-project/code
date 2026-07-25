using System.Security.Cryptography.X509Certificates;

namespace Fullerene.Signer.Infrastructure.Abstractions;

public interface IApkSigningCertificateGenerator
{
    X509Certificate2 CreateCertificate(string androidAppId);
}