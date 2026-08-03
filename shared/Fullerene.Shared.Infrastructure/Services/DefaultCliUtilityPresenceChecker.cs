using CliWrap;
using Fullerene.Shared.Common.Abstractions;
using Fullerene.Shared.Domain.Exceptions;

namespace Fullerene.Shared.Infrastructure.Services;

public abstract class DefaultCliUtilityPresenceChecker : IStartupTask
{
    private readonly string _command;
    private readonly string _utilityName;

    public DefaultCliUtilityPresenceChecker(string command, string? utilityName = null)
    {
        _command = command;
        _utilityName = utilityName ?? command;
    }

    public async Task ExecuteAsync(CancellationToken ct)
    {
        try
        {
            await Cli.Wrap(_command)
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync(ct);
        }
        catch (Exception e)
        {
            throw new InternalException($"{_utilityName} is required to run application. Error: {e.Message}");
        }
    }
}