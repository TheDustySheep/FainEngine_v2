using Silk.NET.OpenGL;
using SkiaSharp;

namespace FainEngine_v2.Rendering.Materials;

public class Texture2D : Texture
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    protected override TextureTarget Target => TextureTarget.Texture2D;
    private PixelFormat _pixelFormat;
    private PixelType _pixelType;

    public Texture2D(
        string path,
        WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode = FilterModes.Nearest,
        MipMapModes mipMapMode = MipMapModes.None) : base(wrapMode, filterMode, mipMapMode)
    {
        _pixelFormat = PixelFormat.Rgba;
        _pixelType   = PixelType.UnsignedByte;

        using var bitmap = SKBitmap.Decode(path);

        if (bitmap == null)
            throw new Exception("Failed to decode raw image data");

        Width = bitmap.Width;
        Height = bitmap.Height;

        UploadBitmap(bitmap);
        SetParameters();
    }

    public Texture2D(
        ReadOnlySpan<byte> data,
        WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode = FilterModes.Nearest,
        MipMapModes mipMapMode = MipMapModes.None) : base(wrapMode, filterMode, mipMapMode)
    {
        using var bitmap = SKBitmap.Decode(data);

        if (bitmap == null)
            throw new Exception("Failed to decode raw image data");

        _pixelFormat = PixelFormat.Rgba;
        _pixelType   = PixelType.UnsignedByte;

        Width  = bitmap.Width;
        Height = bitmap.Height;


        UploadBitmap(bitmap);
        SetParameters();
    }

    public Texture2D(
        int width,
        int height,
        SizedInternalFormat internalFormat = SizedInternalFormat.Rgba8,
        PixelFormat pixelFormat = PixelFormat.Rgba,
        PixelType pixelType     = PixelType.UnsignedByte,
        WrappingModes wrapMode  = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode  = FilterModes.Nearest,
        MipMapModes mipMapMode  = MipMapModes.None) : base(wrapMode, filterMode, mipMapMode)
    {
        _pixelFormat = pixelFormat;
        _pixelType = pixelType;
        Width = width;
        Height = height;

        _gl.TextureStorage2D(
            _handle,
            1,
            internalFormat,
            (uint)width,
            (uint)height);

        SetParameters();
    }

    private Span<byte> FlipVertically(ReadOnlySpan<byte> src, int width, int height)
    {
        if (_pixelFormat != PixelFormat.Rgba)
            throw new InvalidOperationException("FlipVertically only supports RGBA format.");

        if (_pixelType != PixelType.UnsignedByte)
            throw new InvalidOperationException("FlipVertically only supports UnsignedByte pixel type.");

        int stride = width * 4; // Assuming RGBA format
        Span<byte> dst = new byte[src.Length];
        for (int y = 0; y < height; y++)
        {
            int srcIndex = y * stride;
            int dstIndex = (height - y - 1) * stride;
            src.Slice(srcIndex, stride)
               .CopyTo(dst.Slice(dstIndex, stride));
        }
        return dst;
    }

    private void UploadBitmap(SKBitmap bitmap)
    {
        _gl.TextureStorage2D(
            _handle,
            1,
            SizedInternalFormat.Rgba8,
            (uint)bitmap.Width,
            (uint)bitmap.Height
        );

        ReadOnlySpan<byte> pixels = FlipVertically(bitmap.Bytes, bitmap.Width, bitmap.Height);

        _gl.TextureSubImage2D(
            _handle,
            0,
            0, 0,
            (uint)bitmap.Width,
            (uint)bitmap.Height,
            _pixelFormat,
            _pixelType,
            pixels
        );
    }

    public void SaveToPng(string filePath)
    {
        byte[] pixels = new byte[Width * Height * 4];

        _gl.GetTextureSubImage(
            _handle,
            0,
            0, 0, 0,
            (uint)Width,
            (uint)Height,
            1,
            _pixelFormat,
            _pixelType,
            pixels.AsSpan()
        );


        using var bitmap = new SKBitmap(
            new SKImageInfo(
                Width,
                Height,
                SKColorType.Rgba8888,
                SKAlphaType.Premul));

        var flipped = FlipVertically(pixels, Width, Height);
        unsafe
        {
            fixed (byte* ptr = flipped)
            {
                bitmap.SetPixels((IntPtr)ptr);

                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                File.WriteAllBytes(filePath, data.ToArray());
            }
        }
    }

    public void FrameBufferTexture(FramebufferAttachment attachment)
    {
        _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, Target, _handle, 0);
    }
}
