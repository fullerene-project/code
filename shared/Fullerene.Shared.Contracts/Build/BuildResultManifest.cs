using Fullerene.Shared.Domain.Models;

namespace Fullerene.Shared.Contracts.Build;

public sealed class NixBuildOutputManifest
{
    public required ICollection<NixBuildOutputEntry> Entries { get; init; }
    public required int ReleaseChannel { get; init; }
}

public sealed class NixBuildOutputEntry
{
    public required string FileName { get; init; }
    public required string FileSha256 { get; init; }
    public required long FileSizeBytes { get; init; }
    public required int ArtifactType { get; init; }
    public required int MinApiLevel { get; init; }
    public required int TargetApiLevel { get; init; }
    public required int VersionCode { get; init; }

    public required string? SplitId { get; init; }
    public required string? ModuleName { get; init; }
    public required ICollection<int>? CpuArchitectures { get; init; }
    public required int? SingleCpuArchitecture { get; init; }
    public required int? DensityAlias { get; init; }
    public required int? DensityDpi { get; init; }
    public required string? LanguageTargeting { get; init; }
    public required int? DeliveryType { get; init; }
    public required int? AssetModuleType { get; init; }
    public required int? TextureCompressionFormat { get; init; }
}

public sealed class BuildResultManifest
{
    public required ICollection<BuildResultEntry> Entries { get; init; }
    public required ReleaseChannel ReleaseChannel { get; init; }
}

public sealed class BuildResultEntry
{
    public required string UnsignedApkStorageKey { get; init; }
    public required string FileName { get; init; }
    public required string FileSha256 { get; init; }
    public required long FileSizeBytes { get; init; }
    public required ArtifactType ArtifactType { get; init; }
    public required int MinApiLevel { get; init; }
    public required int TargetApiLevel { get; init; }
    public required int VersionCode { get; init; }

    public required string? SplitId { get; init; }
    public required string? ModuleName { get; init; }
    public required ICollection<CpuArchitecture>? CpuArchitectures { get; init; }
    public required CpuArchitecture? SingleCpuArchitecture { get; init; }
    public required ScreenDensityAlias? DensityAlias { get; init; }
    public required int? DensityDpi { get; init; }
    public required string? LanguageTargeting { get; init; }
    public required DeliveryType? DeliveryType { get; init; }
    public required AssetModuleType? AssetModuleType { get; init; }
    public required TextureCompressionFormat? TextureCompressionFormat { get; init; }
}