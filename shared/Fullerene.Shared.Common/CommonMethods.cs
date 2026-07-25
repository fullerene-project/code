using System.Security.Cryptography;

namespace Fullerene.Shared.Common;

public static class CommonMethods
{
    public static async Task<string> GetFileSha256Async(string fullPath, CancellationToken ct)
    {
        await using var stream = File.OpenRead(fullPath);
        var sha256Bytes = await SHA256.HashDataAsync(stream, ct);
        return Convert.ToHexString(sha256Bytes);
    }
}