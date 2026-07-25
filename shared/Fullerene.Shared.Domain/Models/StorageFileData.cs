namespace Fullerene.Shared.Domain.Models;

public sealed class StorageFileData : FileData
{
    public required string FileStorageKey { get; init; }
}