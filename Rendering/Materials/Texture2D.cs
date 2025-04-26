using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FainEngine_v2.Rendering.Materials;

public class Texture2D : Texture
{
    protected override TextureTarget Target => TextureTarget.Texture2D;

    public unsafe Texture2D(
        GL gl,
        string path,
        WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode = FilterModes.Nearest,
        MipMapModes mipMapMode = MipMapModes.None) : base(gl, wrapMode, filterMode, mipMapMode)
    {
        using (var img = Image.Load<Rgba32>(path))
        {
            gl.TexImage2D(Target, 0, InternalFormat.Rgba8, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

            img.Mutate(i => i.RotateFlip(RotateMode.Rotate180, FlipMode.Horizontal));

            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    fixed (void* data = accessor.GetRowSpan(y))
                    {
                        gl.TexSubImage2D(Target, 0, 0, y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                    }
                }
            });
        }

        SetParameters();
    }

    public unsafe Texture2D(
    GL gl,
    Image<Rgba32> img,
    WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
    FilterModes filterMode = FilterModes.Nearest,
    MipMapModes mipMapMode = MipMapModes.None) : base(gl, wrapMode, filterMode, mipMapMode)
    {
        gl.TexImage2D(Target, 0, InternalFormat.Rgba8, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

        img.Mutate(i => i.RotateFlip(RotateMode.Rotate180, FlipMode.Horizontal));

        img.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                fixed (void* data = accessor.GetRowSpan(y))
                {
                    gl.TexSubImage2D(Target, 0, 0, y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                }
            }
        });

        SetParameters();
    }

    public unsafe Texture2D(
        GL gl,
        int width,
        int height,
        InternalFormat internalFormat = InternalFormat.Rgba8,
        PixelFormat pixelFormat = PixelFormat.Rgba,
        PixelType pixelType = PixelType.UnsignedByte,
        WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode = FilterModes.Nearest,
        MipMapModes mipMapMode = MipMapModes.None) : base(gl, wrapMode, filterMode, mipMapMode)
    {
        _gl = gl;
        Bind();
        gl.TexImage2D(
            Target,
            0,
            internalFormat,
            (uint)width,
            (uint)height,
            0,
            pixelFormat,
            pixelType,
            null);

        SetParameters();
    }

    public void FrameBufferTexture(FramebufferAttachment attachment)
    {
        _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, Target, _handle, 0);
    }
}
