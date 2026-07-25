using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Dtos;

public sealed class ClientDeviceInfo
{
    public required CpuArchitecture CpuArchitecture { get; init; }
    public required int ApiVersion { get; init; }
    public required string[] Locales { get; init; }
    public required TextureCompressionFormat[] TextureCompressionFormats { get; init; }
    public int? ScreenDensityDpi { get; init; }
    public ScreenDensityAlias? ScreenDensityAlias { get; init; }
}