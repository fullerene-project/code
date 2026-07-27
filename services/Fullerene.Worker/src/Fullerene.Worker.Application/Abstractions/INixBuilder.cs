namespace Fullerene.Worker.Application.Abstractions;

/// <summary>
/// Defines a service for building Nix packages
/// </summary>
public interface INixBuilder
{
    /// <summary>
    /// Builds the specified nix package and places the result in the specified folder
    /// </summary>
    /// <param name="resultDir">The directory where the build result will be placed</param>
    /// <returns></returns>
    Task StartNixPackageBuildAsync(string nixFlakeUrl,
        string packageName, string resultDir, CancellationToken ct);
}