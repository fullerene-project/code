namespace Fullerene.Shared.Domain.Models;

public class DownloadableFileData
{
    public required string DownloadUrl { get; init; }
    public required string FileName { get; init; }
    public required string FileSha256 { get; init; }
    public required long FileSizeBytes { get; init; }
}