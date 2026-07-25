using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Domain.Models;

public sealed class ScreenDensity
{
    public int? Dpi { get; }
    public ScreenDensityAlias? Alias { get; }

    private ScreenDensity(int? dpi, ScreenDensityAlias? alias)
    {
        if ((dpi is null && alias is null) ||
            (dpi is not null && alias is not null))
            throw new Exception("Only one of Alias or DPI can be specified.");

        Dpi = dpi;
        Alias = alias;
    }

    public static ScreenDensity FromDpi(int? dpi)
    {
        return new ScreenDensity(dpi, null);
    }

    public static ScreenDensity FromAlias(ScreenDensityAlias alias)
    {
        return new ScreenDensity(null, alias);
    }

    public static ScreenDensity FromBoth(int? dpi, ScreenDensityAlias? alias)
    {
        if (dpi is null && alias is not null)
            return FromAlias((ScreenDensityAlias)alias);
        if (dpi is not null && alias is null)
            return FromDpi(dpi);

        throw new Exception("Only one of Alias or DPI can be specified.");
    }

    public void Match(Action<ScreenDensityAlias> onAlias, Action<int> onDpi)
    {
        if (Alias is ScreenDensityAlias alias)
        {
            onAlias(alias);
            return;
        }
        if (Dpi is int dpi)
        {
            onDpi(dpi);
            return;
        }

        throw new Exception("Only one of Alias or DPI can be specified.");
    }
}