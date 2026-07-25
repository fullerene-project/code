namespace Fullerene.Manager.Application.Abstractions;

public interface INixFlakeUrlFormatter
{
    string FormatNixFlakeUrl(string gitRepositoryUrl, string gitCommitHash);
}