using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Application.Extensions;
using Fullerene.Shared.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fullerene.Manager.Application.Cqrs.Queries;

public sealed class GetAndroidAppPackageVersionsQuery
{
    public Guid[]? AndroidAppPackageIds { get; init; }
    public Guid[]? AndroidAppPackageVersionIds { get; init; }
    public ReleaseChannel[]? ReleaseChannels { get; init; }
    public int[]? BaseVersionCodes { get; init; }
    public int? MinBaseVersionCode { get; init; }
    public int? MaxBaseVersionCode { get; init; }
    
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public sealed class GetAndroidAppPackageVersionsQueryHandler(
    IApplicationContext context)
{
    public static readonly int MaxPageSize = 20;
    
    public async Task<IEnumerable<AndroidAppPackageVersionDto>> Handle(
        GetAndroidAppPackageVersionsQuery query, CancellationToken ct)
    {
        if (query.PageSize > MaxPageSize)
            throw new Exception($"Page size can not be greater than {MaxPageSize}");
        
        return await context.AndroidAppPackageVersions
            .AsNoTracking()
            .WhereIf(
                query.AndroidAppPackageVersionIds.NotNullOrEmpty(),
                ver => query.AndroidAppPackageVersionIds.Contains(ver.Id))
            .WhereIf(
                query.AndroidAppPackageIds.NotNullOrEmpty(),
                ver => query.AndroidAppPackageIds.Contains(ver.AndroidAppPackage.Id))
            .WhereIf(
                query.ReleaseChannels.NotNullOrEmpty(),
                ver => query.ReleaseChannels.Contains(ver.ReleaseChannel))
            .WhereIf(
                query.BaseVersionCodes.NotNullOrEmpty(),
                ver => query.BaseVersionCodes.Contains(ver.BaseVersionCode))
            .WhereIf(
                query.MinBaseVersionCode is not null,
                ver => query.MinBaseVersionCode <= ver.BaseVersionCode)
            .WhereIf(
                query.MaxBaseVersionCode is not null,
                ver => query.MaxBaseVersionCode >= ver.BaseVersionCode)
            .OrderByDescending(art => art.AppVersionReleaseDate)
            .ThenBy(art => art.Id)
            .Page(query.Page, query.PageSize)
            .Select(ver =>
                new AndroidAppPackageVersionDto
                {
                    Id = ver.Id,
                    NixPackageRepoId = ver.NixPackageRepoId,
                    CommitHash = ver.CommitHash,
                    NixPackageName = ver.NixPackageName,
                    AndroidApplicationId = ver.AndroidApplicationId,
                    AppVersionString = ver.AppVersionString,
                    BaseVersionCode = ver.BaseVersionCode,
                    NixPackageRevision = ver.NixPackageRevision,
                    NixDerivationHash = ver.NixDerivationHash,
                    ReleaseChannel = ver.ReleaseChannel,
                    AppVersionReleaseDate = ver.AppVersionReleaseDate,
                    AppLogoUrl = ver.AppLogoUrl,
                    AppName = ver.AppName,
                    AppSummary = ver.AppSummary,
                    AppDescription = ver.AppDescription,
                    AppLicense = ver.AppLicense,
                    ReleaseNotes = ver.ReleaseNotes,
                })
            .ToArrayAsync(ct);
    }
}