using Fullerene.Manager.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Fullerene.Manager.Application.Cqrs.Commands;

public sealed class UpdateAllNixReposCommand;

public sealed class UpdateAllNixReposCommandHandler(
    IApplicationContext context,
    IMessageBus messageBus)
{
    public async Task Handle(UpdateAllNixReposCommand command, CancellationToken ct)
    {
        var allRepoIds =
            await context.NixPackageRepos
                .Select(x => x.Id)
                .ToArrayAsync(ct);

        var updateReposCommand = new UpdateNixReposCommand { NixRepoIds = allRepoIds };

        await messageBus.InvokeAsync<UpdateNixReposCommand>(updateReposCommand, ct);
    }
}