using Fullerene.Manager.Application.Abstractions;

namespace Fullerene.Manager.Infrastructure.Services;

public sealed class NixFlakeUrlFormatter : INixFlakeUrlFormatter
{
    public string FormatNixFlakeUrl(string gitRepositoryUrl, string gitCommitHash)
    {
        var querySeparator = gitRepositoryUrl.Contains('?') ? '&' : '?';
        return $"git+{gitRepositoryUrl}{querySeparator}rev={gitCommitHash}";
    }
}