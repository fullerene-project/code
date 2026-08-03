using Fullerene.Manager.Application.Abstractions;
using Fullerene.Manager.Application.Dtos;
using Fullerene.Shared.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Fullerene.Manager.Application.Cqrs.Commands;

public sealed class StartTrackingAndroidAppCommand
{
    public required Guid AndroidAppId { get; init; }
};

public sealed class StartTrackingAndroidAppCommandHandler(
    IMessageBus messageBus,
    IApplicationContext context)
{
    public async Task Handle(StartTrackingAndroidAppCommand command, CancellationToken ct)
    {
        var app = await context.AndroidAppPackages
            .FirstOrDefaultAsync(x => x.Id == command.AndroidAppId, ct);

        if (app is null)
            throw new NotFoundException($"App with id: \"{command.AndroidAppId}\" does not exist");

        if (app.IsTracked)
            throw new ConflictException($"App with id: \"{command.AndroidAppId}\" is already tracked");

        app.IsTracked = true;

        await context.SaveChangesAsync(ct);

        await messageBus.PublishAsync(new ResolveVersionsToBuildCommand
        {
            PackageIdentifiers = [new CombinedNixAppPackageIdentifier(app.NixPackageRepoId, app.AndroidApplicationId, app.NixPackageName)]
        });
    }
}