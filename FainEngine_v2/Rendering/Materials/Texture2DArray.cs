using Silk.NET.OpenGL;
using StbImageSharp;

namespace FainEngine_v2.Rendering.Materials;

public class Texture2DArray : Texture
{
    protected override TextureTarget Target => TextureTarget.Texture2DArray;

    public unsafe Texture2DArray(
        string path,
        uint atlasSize = 16,
        WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode = FilterModes.Nearest,
        MipMapModes mipMapMode = MipMapModes.None) : base(wrapMode, filterMode, mipMapMode)
    {
        using var stream = File.OpenRead(path);
        var img = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        int tileWidth = img.Width / (int)atlasSize;
        int tileHeight = img.Height / (int)atlasSize;
        int depth = (int)(atlasSize * atlasSize);

        _GL.TextureStorage3D(
            _handle,
            1,
            SizedInternalFormat.Rgba8,
            (uint)tileWidth,
            (uint)tileHeight,
            (uint)depth
        );

        UploadAtlas(img.Data, tileWidth, tileHeight, (int)atlasSize);

        SetParameters();
    }

    private unsafe void UploadAtlas(byte[] data, int tileWidth, int tileHeight, int atlasSize)
    {
        int tilesPerRow = atlasSize;
        int depth = atlasSize * atlasSize;

        int atlasWidth = tileWidth * atlasSize;
        int stride = atlasWidth * 4;

        for (int tileIndex = 0; tileIndex < depth; tileIndex++)
        {
            int tileX = tileIndex % tilesPerRow;
            int tileY = tileIndex / tilesPerRow;

            for (int y = 0; y < tileHeight; y++)
            {
                int srcY = tileY * tileHeight + (tileHeight - 1 - y);
                int srcOffset = (srcY * atlasWidth + tileX * tileWidth) * 4;

                fixed (byte* ptr = &data[srcOffset])
                {
                    _GL.TextureSubImage3D(
                        _handle,
                        0,
                        0,
                        y,
                        tileIndex,
                        (uint)tileWidth,
                        1,
                        1,
                        PixelFormat.Rgba,
                        PixelType.UnsignedByte,
                        ptr
                    );
                }
            }
        }
    }
}
