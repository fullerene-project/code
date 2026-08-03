using Fullerene.Shared.Domain.Exceptions;
using Fullerene.Signer.Infrastructure.Abstractions;
using Fullerene.Signer.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Fullerene.Signer.Infrastructure.Services;

public sealed class MasterSeedProvider(
    IOptions<SigningSettings> signingSettings) : IMasterSeedProvider
{
    public byte[] GetMasterSeed()
    {
        var base64MasterSeed = signingSettings.Value.MasterSeedBase64;
        try
        {
            return Convert.FromBase64String(base64MasterSeed);
        }
        catch (Exception e)
        {
            throw new InternalException($"Invalid base64 master seed. Error Message: \"{e.Message}\"");
        }
    }
}