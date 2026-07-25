using Fullerene.Shared.Domain.Models;

namespace Fullerene.Manager.Application.Dtos;

public sealed class SignedArtifactDownloadData
{
    public required DownloadableFileData ApkFileData { get; set; }
    public required DownloadableFileData? ApkIdSigFileData { get; set; }
}