using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Fullerene.Signer.Application.Abstractions;
using Fullerene.Signer.Infrastructure.Abstractions;
using Fullerene.Signer.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Fullerene.Signer.Infrastructure.Services;

public sealed class ApkSigningCertificateGenerator : IApkSigningCertificateGenerator
{
    private readonly SignerIdentitySettings _signerIdentitySettings;
    private readonly IECDsaDeriver _ecdsaDeriver;
    private readonly IPerAppPrivateKeyDeriver _perAppPrivateKeyDeriver;

    public ApkSigningCertificateGenerator(
        IOptions<SignerIdentitySettings> signerIdentitySettings,
        IECDsaDeriver ecdsaDeriver,
        IPerAppPrivateKeyDeriver perAppPrivateKeyDeriver)
    {
        _signerIdentitySettings = signerIdentitySettings.Value;
        _ecdsaDeriver = ecdsaDeriver;
        _perAppPrivateKeyDeriver = perAppPrivateKeyDeriver;
    }

    public X509Certificate2 CreateCertificate(string androidAppId)
    {
        var privateKey = _perAppPrivateKeyDeriver.DerivePrivateKey(androidAppId);
        using var ecdsa = _ecdsaDeriver.DeriveFromPrivateKey(privateKey);

        var request = new CertificateRequest(
            $"CN={_signerIdentitySettings.CommonName}, O={_signerIdentitySettings.Organization}, OU={androidAppId}",
            ecdsa,
            HashAlgorithmName.SHA256);

        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(
                critical: true,
                certificateAuthority: false,
                hasPathLengthConstraint: false,
                pathLengthConstraint: 0));

        var notBefore = DateTimeOffset.UtcNow.AddDays(-1);
        var notAfter = notBefore.AddYears(30);

        return request.CreateSelfSigned(notBefore, notAfter);
    }
}