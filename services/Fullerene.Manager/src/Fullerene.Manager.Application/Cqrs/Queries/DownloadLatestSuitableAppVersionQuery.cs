using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Application.Extensions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Manager.Domain.Models.ConcreteArtifactDataRecords;
using Fullerene.Manager.Domain.Models.WorkflowEvents;
using Fullerene.Shared.Common.Abstractions.Storage;
using Fullerene.Shared.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fullerene.Manager.Application.Cqrs.Queries;

public sealed class DownloadLatestSuitableAppVersionQuery
{
    public required Guid AppId { get; init; }
    public required ClientDeviceInfo? ClientDeviceInfo { get; init; }
    public required ReleaseChannel[] ReleaseChannels { get; init; }
    public required int? CurrentBaseVersionCode { get; init; }
    public required bool StandaloneApkOnly { get; init; }
}

public sealed class DownloadLatestSuitableAppVersionQueryHandler(
    IApplicationContext context,
    ISignedApkPublicPresignedUrlProvider signedApkPublicPresignedUrlProvider)
{
    public async Task<IEnumerable<SignedArtifactDownloadData>> Handle(
        DownloadLatestSuitableAppVersionQuery query,
        CancellationToken ct)
    {
        var artifacts = (await context.AndroidAppPackageVersions
            .AsNoTracking()
            .Include(ver => ver.BuildWorkflows)
                .ThenInclude(bw => bw.Artifacts)
                    .ThenInclude(art => art.ArtifactDataRecord)
            .Include(ver => ver.BuildWorkflows)
                .ThenInclude(bw => bw.WorkflowEvents)
            .Where(ver =>
                ver.AndroidAppPackage.Id == query.AppId &&
                query.ReleaseChannels.Contains(ver.ReleaseChannel) &&
                ver.BuildWorkflows.Any(bw => bw.WorkflowEvents
                    .Any(we => we.EventType == WorkflowEventType.BuildSucceeded)))
            .WhereIf(
                query.CurrentBaseVersionCode is not null,
                ver => ver.BaseVersionCode > query.CurrentBaseVersionCode)
            .OrderByDescending(ver => ver.BaseVersionCode)
            .ThenByDescending(ver => ver.AppVersionReleaseDate)
            .Select(ver => ver.BuildWorkflows
                .Where(bw =>
                    bw.WorkflowEvents.Any(we => we.EventType == WorkflowEventType.BuildSucceeded) &&
                    bw.Artifacts.Any() &&
                    bw.Artifacts.Count(art => art.IsSigned) == bw.Artifacts.Count(art => !art.IsSigned) &&
                    (!query.StandaloneApkOnly || bw.Artifacts.Any(art =>
                        art.ArtifactDataRecord is StandaloneSingleAbiArtifactDataRecord ||
                        art.ArtifactDataRecord is StandaloneUniversalArtifactDataRecord)))
                .SelectMany(bw => bw.Artifacts)
                .Where(art => art.IsSigned))
            .FirstOrDefaultAsync(ct))
            ?.ToArray();

        if (artifacts is null)
        {
            throw new Exception("No suitable updates found");
        }

        if (!query.StandaloneApkOnly && query.ClientDeviceInfo is not null)
        {
            var bestSplits = FindBestSplits(artifacts, query.ClientDeviceInfo);

            if (bestSplits is not null)
                return bestSplits.Select(signedApkPublicPresignedUrlProvider.GetDownloadData);
        }

        var clientCpuArch = query.ClientDeviceInfo?.CpuArchitecture;

        if (clientCpuArch is not null)
        {
            var singleAbiArtifactDto = artifacts.FirstOrDefault(art =>
                art.ArtifactDataRecord is StandaloneSingleAbiArtifactDataRecord record &&
                record.CpuArchitecture == clientCpuArch);

            if (singleAbiArtifactDto is not null)
                return [signedApkPublicPresignedUrlProvider.GetDownloadData(singleAbiArtifactDto)];
        }

        var universalArtifactDto = artifacts.FirstOrDefault(art =>
                                       art.ArtifactDataRecord.ArtifactType == ArtifactType.StandaloneUniversal)
                                   ?? throw new Exception("No suitable updates found");

        return [signedApkPublicPresignedUrlProvider.GetDownloadData(universalArtifactDto)];
    }

    private IEnumerable<Artifact>? FindBestSplits(
        IEnumerable<Artifact> artifacts, ClientDeviceInfo clientDeviceInfo)
    {
        var neededArtifacts = new HashSet<Artifact>();

        var neededModuleNames = new HashSet<string>();

        var masterArtifacts = artifacts
            .Where(art => art.ArtifactDataRecord is MasterArtifactSplitDataRecord masterRecord)
            .ToArray();

        if (masterArtifacts.Any(art => art.ArtifactDataRecord is BaseArtifactSplitDataRecord))
        {
            foreach (var masterArtifact in masterArtifacts)
            {
                neededArtifacts.Add(masterArtifact);
                neededModuleNames.Add((masterArtifact.ArtifactDataRecord as MasterArtifactSplitDataRecord).ModuleName);
            }
        }
        else return null;

        var configSplitsByTypeAndModuleName = artifacts
            .Except(masterArtifacts)
            .Where(art =>
                art.ArtifactDataRecord is ArtifactSplitDataRecord split &&
                neededModuleNames.Contains(split.ModuleName))
            .GroupBy(x => new { x.ArtifactDataRecord.ArtifactType, (x.ArtifactDataRecord as ArtifactSplitDataRecord).ModuleName })
            .ToDictionary(x => x.Key, x => x.ToArray());

        foreach (var configSplits in configSplitsByTypeAndModuleName)
        {
            switch (configSplits.Key.ArtifactType)
            {
                case ArtifactType.AbiSplit:
                    {
                        var typed = configSplits.Value.Select(art =>
                            new TypedArtifact<AbiArtifactSplitDataRecord>(art,
                                art.ArtifactDataRecord as AbiArtifactSplitDataRecord));

                        var neededAbi = typed.FirstOrDefault(art =>
                            art.DataRecord.CpuArchitecture == clientDeviceInfo.CpuArchitecture);

                        if (neededAbi is null) return null;

                        neededArtifacts.Add(neededAbi.Artifact);

                        break;
                    }
                case ArtifactType.DensitySplit:
                    {
                        var typed = configSplits.Value.Select(art =>
                            new TypedArtifact<DensityArtifactSplitDataRecord>(art,
                                art.ArtifactDataRecord as DensityArtifactSplitDataRecord));

                        var clientDpi = clientDeviceInfo.ScreenDensityDpi ??
                                        (clientDeviceInfo.ScreenDensityAlias is not null
                                        ? GetDensityDpiFromAlias((ScreenDensityAlias)clientDeviceInfo.ScreenDensityAlias)
                                        : throw new ArgumentNullException(nameof(clientDeviceInfo.ScreenDensityDpi)));

                        var dpis = typed
                            .Select(art =>
                            {
                                int artifactDpi = 0;

                                art.DataRecord.Density.Match(
                                    alias => artifactDpi = GetDensityDpiFromAlias(alias),
                                    dpi => artifactDpi = dpi);

                                return new { artifactDpi, art };
                            })
                            .OrderBy(art => art.artifactDpi)
                            .ToArray();

                        Artifact? chosenSplit = null;
                        for (var i = 0; i < dpis.Length; i++)
                        {
                            var currentArtifact = dpis[i];
                            if (currentArtifact.artifactDpi <= clientDpi && i == dpis.Length - 1)
                            {
                                chosenSplit = currentArtifact.art.Artifact;
                                break;
                            }
                            if (currentArtifact.artifactDpi >= clientDpi)
                            {
                                chosenSplit = currentArtifact.art.Artifact;
                                break;
                            }
                        }

                        if (chosenSplit is null) return null;

                        neededArtifacts.Add(chosenSplit);
                        break;
                    }
                case ArtifactType.LanguageSplit:
                    {
                        var typed = configSplits.Value.Select(art =>
                            new TypedArtifact<LanguageArtifactSplitDataRecord>(art,
                                art.ArtifactDataRecord as LanguageArtifactSplitDataRecord));

                        var neededLanguages = typed.Where(art =>
                            clientDeviceInfo.Locales.Contains(art.DataRecord.LanguageTargeting));

                        if (!neededLanguages.Any()) return null;

                        foreach (var neededLanguage in neededLanguages)
                        {
                            neededArtifacts.Add(neededLanguage.Artifact);
                        }

                        break;
                    }
                default: throw new ArgumentOutOfRangeException();
            }
        }

        var assetSplitsByModuleNames = artifacts
            .Where(art => art.ArtifactDataRecord.ArtifactType == ArtifactType.AssetsSplit)
            .Select(art =>
                new TypedArtifact<AssetsArtifactSplitDataRecord>(art,
                    art.ArtifactDataRecord as AssetsArtifactSplitDataRecord))
            .GroupBy(x => x.DataRecord.ModuleName)
            .ToDictionary(x => x.Key, x => x.AsEnumerable());

        foreach (var assetSplitsByModuleName in assetSplitsByModuleNames)
        {
            var selectedTcf = assetSplitsByModuleName.Value
                .Where(art =>
                    art.DataRecord.TextureCompressionFormat is not null &&
                    clientDeviceInfo.TextureCompressionFormats.Contains((TextureCompressionFormat)art.DataRecord.TextureCompressionFormat))
                .OrderBy(art =>
                    GetTextureCompressionFormatPriority((TextureCompressionFormat)art.DataRecord.TextureCompressionFormat))
                .FirstOrDefault()
                ?.DataRecord.TextureCompressionFormat;

            foreach (var assetSplit in assetSplitsByModuleName.Value)
            {
                var record = assetSplit.DataRecord;
                if (selectedTcf == record.TextureCompressionFormat && (
                        record.LanguageTargeting is null ||
                        clientDeviceInfo.Locales.Contains(record.LanguageTargeting)))
                {
                    neededArtifacts.Add(assetSplit.Artifact);
                }
            }
        }

        return neededArtifacts;
    }

    private static int GetTextureCompressionFormatPriority(TextureCompressionFormat format)
    {
        return format switch
        {
            TextureCompressionFormat.ASTC => 1,
            TextureCompressionFormat.ETC2 => 2,
            TextureCompressionFormat.S3TC => 3,
            TextureCompressionFormat.DXT1 => 4,
            TextureCompressionFormat.LATC => 5,
            TextureCompressionFormat.THREE_DC => 6,
            TextureCompressionFormat.ATC => 7,
            TextureCompressionFormat.PVRTC => 8,
            TextureCompressionFormat.ETC1_RGB8 => 9,
            TextureCompressionFormat.PALETTED => 10,
            TextureCompressionFormat.UNCOMPRESSED => 11,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    private static int GetDensityDpiFromAlias(ScreenDensityAlias alias)
    {
        return alias switch
        {
            ScreenDensityAlias.NODPI => 0,
            ScreenDensityAlias.LDPI => 120,
            ScreenDensityAlias.MDPI => 160,
            ScreenDensityAlias.TVDPI => 213,
            ScreenDensityAlias.HDPI => 240,
            ScreenDensityAlias.XHDPI => 320,
            ScreenDensityAlias.XXHDPI => 480,
            ScreenDensityAlias.XXXHDPI => 640,
            _ => throw new ArgumentOutOfRangeException(nameof(alias), alias, null)
        };
    }

    private record TypedArtifact<TRecord>(Artifact Artifact, TRecord DataRecord) where TRecord : ArtifactDataRecord;
}