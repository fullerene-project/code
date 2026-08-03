using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Domain.Models;
using Fullerene.Shared.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Fullerene.Manager.Application.Cqrs.Commands;

public sealed class AddNixRepoCommand
{
    public required string Name { get; init; }
    public required string GitRepositoryUrl { get; init; }
}

public sealed class AddNixRepoCommandHandler(
    IApplicationContext context,
    IMessageBus messageBus,
    ILogger<AddNixRepoCommandHandler> logger)
{
    public async Task<NixPackageRepo> Handle(AddNixRepoCommand command, CancellationToken ct)
    {
        if (await context.NixPackageRepos.AnyAsync(x => x.Name == command.Name, ct))
        {
            throw ValidationException.FromSingleError(nameof(command.Name),
                $"Nix package repo with name: \"{command.Name}\" already exists");
        }

        var repo = NixPackageRepo.CreateNew(
            name: command.Name,
            gitRepositoryUrl: command.GitRepositoryUrl);

        context.NixPackageRepos.Add(repo);

        await messageBus.PublishAsync(new UpdateNixReposCommand { NixRepoIds = [repo.Id] });

        await context.SaveChangesAsync(ct);

        return repo;
    }
}