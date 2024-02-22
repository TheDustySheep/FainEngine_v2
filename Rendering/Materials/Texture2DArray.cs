using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Materials;

public class Texture2DArray : Texture
{
    protected override TextureTarget Target => TextureTarget.Texture2DArray;

    public unsafe Texture2DArray(
        GL gl,
        string path,
        uint atlasSize = 16,
        WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode = FilterModes.Nearest,
        MipMapModes mipMapMode = MipMapModes.None) : base(gl, wrapMode, filterMode, mipMapMode)
    {
        using (var img = Image.Load<Rgba32>(path))
        {
            uint width = (uint)img.Width / atlasSize;
            uint height = (uint)img.Height / atlasSize;
            uint depth = atlasSize * atlasSize;

            gl.TexImage3D(Target, 0, InternalFormat.Rgba8, width, height, depth, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

            img.Mutate(i => i.Rotate(RotateMode.Rotate90));

            Span<Rgba32> pixels = stackalloc Rgba32[1];
            pixels[0] = Rgba32.ParseHex("FF0000");

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
                                gl.TexSubImage3D(Target, 0, y, 0, i_depth, 1, width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                            }
                        }
                    });
                }
            }
        }

        SetParameters();
    }
}
