namespace Fullerene.Shared.Domain.Models;

public enum TextureCompressionFormat
{
    UNCOMPRESSED = 0,
    ETC1_RGB8 = 1,
    PALETTED = 2,
    THREE_DC = 3,
    ATC = 4,
    LATC = 5,
    DXT1 = 6,
    S3TC = 7,
    PVRTC = 8,
    ASTC = 9,
    ETC2 = 10
}