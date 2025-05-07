using FainEngine_v2.Core;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FainEngine_v2.Rendering.Materials;

public class Texture2D : Texture
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    protected override TextureTarget Target => TextureTarget.Texture2D;

    public Texture2D(
        string path,
        WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode = FilterModes.Nearest,
        MipMapModes mipMapMode = MipMapModes.None) : base(wrapMode, filterMode, mipMapMode)
    {
        using (var img = Image.Load<Rgba32>(path))
        {
            Width = img.Width;
            Height = img.Height;
            Process(img);
        }

        SetParameters();
    }

    public Texture2D(
        Image<Rgba32> img,
        WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode = FilterModes.Nearest,
        MipMapModes mipMapMode = MipMapModes.None) : base(wrapMode, filterMode, mipMapMode)
    {
        Width = img.Width;
        Height = img.Height;

        Process(img);
        SetParameters();
    }

    public unsafe Texture2D(
        int width,
        int height,
        SizedInternalFormat internalFormat = SizedInternalFormat.Rgba8,
        PixelFormat pixelFormat = PixelFormat.Rgba,
        PixelType pixelType = PixelType.UnsignedByte,
        WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode = FilterModes.Nearest,
        MipMapModes mipMapMode = MipMapModes.None) : base(wrapMode, filterMode, mipMapMode)
    {
        Width = width;
        Height = height;

        _gl.TextureStorage2D(
            _handle, 
            1,
            internalFormat, 
            (uint)width, 
            (uint)height);

        //_gl.TexImage2D(
        //    Target,
        //    0,
        //    internalFormat,
        //    (uint)width,
        //    (uint)height,
        //    0,
        //    pixelFormat,
        //    pixelType,
        //    null);

        SetParameters();
    }

    private unsafe void Process(Image<Rgba32> img)
    {
        //_gl.TexImage2D(Target, 0, InternalFormat.Rgba8, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
        _gl.TextureStorage2D(_handle, 1, SizedInternalFormat.Rgb8, (uint)img.Width, (uint)img.Height);

        img.Mutate(i => i.RotateFlip(RotateMode.Rotate180, FlipMode.Horizontal));

        img.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                fixed (void* data = accessor.GetRowSpan(y))
                {
                    _gl.TextureSubImage2D(
                        _handle,
                        0,
                        0, y,
                        (uint)accessor.Width, 1,
                        PixelFormat.Rgba,
                        PixelType.UnsignedByte,
                        data);

                    //_gl.TexSubImage2D(Target, 0, 0, y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                }
            }
        });
    }

    public unsafe void SetData(System.Drawing.Rectangle bounds, byte[] data)
    {
        fixed (byte* ptr = data)
        {
            _gl.TextureSubImage2D(
                _handle,
                level:   0,
                xoffset: bounds.Left,
                yoffset: bounds.Top,
                width:   (uint)bounds.Width,
                height:  (uint)bounds.Height,
                format:  PixelFormat.Rgba,
                type:    PixelType.UnsignedByte,
                pixels:  ptr
            );
        }
    }

    public void FrameBufferTexture(FramebufferAttachment attachment)
    {
        _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, Target, _handle, 0);
    }
}
