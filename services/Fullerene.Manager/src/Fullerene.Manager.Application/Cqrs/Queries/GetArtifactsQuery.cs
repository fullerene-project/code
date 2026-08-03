using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Application.Extensions;
using Fullerene.Manager.Application.Extensions.Mapping;
using Fullerene.Shared.Domain.Exceptions;
using Fullerene.Shared.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fullerene.Manager.Application.Cqrs.Queries;

public sealed class GetArtifactsQuery
{
    public Guid[]? AndroidAppPackageIds { get; init; }
    public Guid[]? BuildWorkflowIds { get; init; }
    public ReleaseChannel[]? ReleaseChannels { get; init; }
    public ArtifactType[]? ArtifactTypes { get; init; }
    public bool? IsSigned { get; init; }

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public sealed class GetArtifactsQueryHandler(IApplicationContext context)
{
    public static readonly int MaxPageSize = 20;

    public async Task<IEnumerable<ArtifactDto>> Handle(GetArtifactsQuery query, CancellationToken ct)
    {
        if (query.PageSize > MaxPageSize)
            throw ValidationException.FromSingleError(nameof(query.Page), $"Page size can not be greater than {MaxPageSize}");

        var artifactDtos = (await context.Artifacts
            .AsNoTracking()
            .Include(art => art.ArtifactDataRecord)
            .WhereIf(
                query.AndroidAppPackageIds.NotNullOrEmpty(),
                art => query.AndroidAppPackageIds.Contains(art.BuildWorkflow.AndroidAppPackageVersion.AndroidAppPackage.Id))
            .WhereIf(
                query.BuildWorkflowIds.NotNullOrEmpty(),
                art => query.BuildWorkflowIds.Contains(art.BuildWorkflowId))
            .WhereIf(
                query.ReleaseChannels.NotNullOrEmpty(),
                art => query.ReleaseChannels.Contains(art.BuildWorkflow.AndroidAppPackageVersion.ReleaseChannel))
            .WhereIf(
                query.ArtifactTypes.NotNullOrEmpty(),
                art => query.ArtifactTypes.Contains(art.ArtifactDataRecord.ArtifactType))
            .WhereIf(
                query.IsSigned is not null,
                art => art.IsSigned == query.IsSigned)
            .OrderBy(art => art.Id)
            .Page(query.Page, query.PageSize)
            .ToArrayAsync(ct))
            .Select(art => art.ToDto());

        return artifactDtos;
    }
}