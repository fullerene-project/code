using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Application.Extensions;
using Fullerene.Manager.Application.Util;
using Microsoft.EntityFrameworkCore;

namespace Fullerene.Manager.Application.Cqrs.Queries;

public sealed class GetAndroidAppPackagesQuery
{
    public Guid[]? AndroidAppPackageIds { get; init; }
    public Guid[]? NixPackageRepoIds { get; init; }
    public string[]? NixPackageNames { get; init; }
    public string[]? AndroidApplicationIds { get; init; }
    public bool? IsTracked { get; init; }
    public string? SearchName { get; init; }

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class GetAndroidAppsQueryHandler(
    IApplicationContext context)
{
    public static readonly int MaxPageSize = 30;

    public async Task<IEnumerable<AndroidAppPackageDto>> Handle(
        GetAndroidAppPackagesQuery query, CancellationToken ct)
    {
        if (query.PageSize > MaxPageSize)
            throw new Exception($"Page size can not be greater than {MaxPageSize}");

        var dbQuery = context.AndroidAppPackages
            .AsNoTracking()
            .Include(x => x.AndroidAppPackageVersions
                .OrderByDescending(y => y.BaseVersionCode)
                .ThenBy(y => y.Id)
                .Take(1))
            .WhereIf(
                query.IsTracked is not null,
                x => x.IsTracked == query.IsTracked)
            .WhereIf(
                query.AndroidAppPackageIds.NotNullOrEmpty(),
                x => query.AndroidAppPackageIds.Contains(x.Id))
            .WhereIf(
                query.NixPackageRepoIds.NotNullOrEmpty(),
                x => query.NixPackageRepoIds.Contains(x.NixPackageRepoId))
            .WhereIf(
                query.NixPackageNames.NotNullOrEmpty(),
                x => query.NixPackageNames.Contains(x.NixPackageName))
            .WhereIf(
                query.AndroidApplicationIds.NotNullOrEmpty(),
                x => query.AndroidApplicationIds.Contains(x.AndroidApplicationId));

        if (query.SearchName is not null)
        {
            dbQuery = dbQuery
                .Where(x => ApplicationDbFunctions.FuzzySimilar(
                    x.AndroidAppPackageVersions.First().AppName,
                    query.SearchName))
                .OrderByDescending(x => ApplicationDbFunctions.FuzzySimilarityDistance(
                    x.AndroidAppPackageVersions.First().AppName,
                    query.SearchName))
                .ThenBy(x => x.Id);
        }
        else dbQuery = dbQuery.OrderByDescending(x => x.Id);

        var appDtos = await dbQuery
            .Page(query.Page, query.PageSize)
            .Select(x => new AndroidAppPackageDto
            {
                Id = x.Id,
                NixPackageRepoId = x.NixPackageRepoId,
                NixPackageName = x.NixPackageName,
                AndroidApplicationId = x.AndroidApplicationId,
                IsTracked = x.IsTracked,
                AppLogoUrl = x.AndroidAppPackageVersions.First().AppLogoUrl,
                AppName = x.AndroidAppPackageVersions.First().AppName,
                AppSummary = x.AndroidAppPackageVersions.First().AppSummary,
                AppDescription = x.AndroidAppPackageVersions.First().AppDescription,
                AppLicense = x.AndroidAppPackageVersions.First().AppLicense
            })
            .ToArrayAsync(ct);

        return appDtos;
    }
}