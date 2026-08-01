using Fullerene.Shared.Domain.Models;

namespace Fullerene.Signer.Application.Dtos;

public sealed class SigningResult
{
    public required string UnsignedApkFullPath { get; set; }
    public required string SignedApkFullPath { get; set; }
    public required string IdSigFileFullPath { get; set; }
}