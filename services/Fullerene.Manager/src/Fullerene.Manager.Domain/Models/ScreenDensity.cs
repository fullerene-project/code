using Fullerene.Shared.Domain.Exceptions;
using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Domain.Models;

public sealed class ScreenDensity
{
    public int? Dpi { get; }
    public ScreenDensityAlias? Alias { get; }

    /// <exception cref="InvariantViolationException">Both dpi and alias is specified or both is null</exception>
    public ScreenDensity(int? dpi, ScreenDensityAlias? alias)
    {
        if (dpi.HasValue == alias.HasValue)
            throw new InvariantViolationException($"Only one of {nameof(Alias)} or {nameof(Dpi)} must be specified.");
        
        Dpi = dpi;
        Alias = alias;
    }

    public static ScreenDensity FromDpi(int dpi)
    {
        return new ScreenDensity(dpi, null);
    }

    public static ScreenDensity FromAlias(ScreenDensityAlias alias)
    {
        return new ScreenDensity(null, alias);
    }

    public void Match(Action<ScreenDensityAlias> onAlias, Action<int> onDpi)
    {
        if (Alias is { } alias)
        {
            onAlias(alias);
            return;
        }
        if (Dpi is { } dpi)
        {
            onDpi(dpi);
            return;
        }

        throw new InvariantViolationException($"Both {nameof(Alias)} and {nameof(Dpi)} are not specified.");
    }
}