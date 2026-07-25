namespace Fullerene.Shared.Domain.Models;

public class FileData
{
    public required string FileName { get; init; }
    public required string FileSha256 { get; init; }
    public required long FileSizeBytes { get; init; }
}