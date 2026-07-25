using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Dtos;

public sealed class ArtifactDataRecordDto
{
    public required ArtifactType ArtifactType { get; init; }
    public required int VersionCode { get; init; }
    public required int MinApiLevel { get; init; }
    public required int TargetApiLevel { get; init; }

    public string? SplitId { get; set; }
    public string? ModuleName { get; set; }
    public IEnumerable<CpuArchitecture>? CpuArchitectures { get; set; }
    public DeliveryType? DeliveryType { get; set; }
    public AssetModuleType? AssetModuleType { get; set; }
    public TextureCompressionFormat? TextureCompressionFormat { get; set; }
    public string? LanguageTargeting { get; set; }
    public ScreenDensityAlias? DensityAlias { get; set; }
    public int? DensityDpi { get; set; }
}