using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
        using (var img = Image.Load<Rgba32>(path))
        {
            uint width = (uint)img.Width / atlasSize;
            uint height = (uint)img.Height / atlasSize;
            uint depth = atlasSize * atlasSize;

            _gl.TextureStorage3D(_handle, 1, SizedInternalFormat.Rgba8, width, height, depth);

            img.Mutate(i => i.Rotate(RotateMode.Rotate90));

            for (int i = 0, i_depth = 0; i < atlasSize; i++)
            {
                for (int j = 0; j < atlasSize; j++, i_depth++)
                {
                    img.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < height; y++)
                        {
                            var slice = accessor.GetRowSpan((int)(j * height + y)).Slice((int)(i * width), (int)width);
                            fixed (void* data = slice)
                            {
                                _gl.TextureSubImage3D(
                                    _handle,
                                    0,
                                    y,
                                    0,
                                    i_depth,
                                    1,
                                    width,
                                    1,
                                    PixelFormat.Rgba,
                                    PixelType.UnsignedByte,
                                    data);
                            }
                        }
                    });
                }
            }
        }

        SetParameters();
    }
}
