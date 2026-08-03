using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Dtos;
using Fullerene.Manager.Application.Extensions;
using Fullerene.Manager.Application.Util;
using Fullerene.Shared.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Fullerene.Manager.Application.Cqrs.Queries;

public sealed class GetNixReposQuery
{
    public string? SearchName { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public sealed class GetNixReposQueryHandler(
    IApplicationContext context)
{
    public static readonly int MaxPageSize = 20;

    public async Task<IEnumerable<NixRepoDto>> Handle(
        GetNixReposQuery query, CancellationToken ct)
    {
        if (query.PageSize > MaxPageSize)
            throw ValidationException.FromSingleError(nameof(query.Page), $"Page size can not be greater than {MaxPageSize}");

        var dbQuery = context.NixPackageRepos.AsNoTracking();

        if (query.SearchName is not null)
        {
            dbQuery = dbQuery
                .Where(x => ApplicationDbFunctions.FuzzySimilar(x.Name, query.SearchName))
                .OrderByDescending(x => ApplicationDbFunctions.FuzzySimilarityDistance(x.Name, query.SearchName))
                .ThenBy(x => x.Id);
        }
        else dbQuery = dbQuery.OrderBy(x => x.Id);

        var repoDtos = await dbQuery
            .Page(query.Page, query.PageSize)
            .Select(x => new NixRepoDto
            {
                Id = x.Id,
                Name = x.Name,
                GitRepositoryUrl = x.GitRepositoryUrl
            })
            .ToArrayAsync(ct);

        return repoDtos;
    }
}