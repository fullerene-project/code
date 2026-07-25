using Fullerene.Manager.Domain.Models;

namespace Fullerene.Manager.Application.Abstractions;

public interface IAndroidAppNixPackageMetaPuller
{
    Task<IEnumerable<AndroidAppNixPackageMeta>?> GetNixPackageMeta(string nixFlakeUrl, CancellationToken ct);
}