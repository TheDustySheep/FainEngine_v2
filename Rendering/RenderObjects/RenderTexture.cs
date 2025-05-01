using FainEngine_v2.Rendering.Materials;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.RenderObjects;
public sealed class RenderTexture : IFrameBuffer, IDisposable
{
    readonly GL _gl;

    public Texture2D ColorTexture => color_tex;
    public Texture2D DepthTexture => depth_tex;

    readonly Texture2D color_tex;
    readonly Texture2D depth_tex;
    readonly FrameBufferObject fbo;

    public readonly int Height;
    public readonly int Width;

    public unsafe RenderTexture(GL gl, int width, int height)
    {
        _gl = gl;
        Width = width;
        Height = height;

        fbo = new FrameBufferObject(_gl);

        color_tex = new Texture2D(
            width,
            height,
            InternalFormat.Rgba,
            PixelFormat.Rgba,
            PixelType.UnsignedByte);

        color_tex.FrameBufferTexture(FramebufferAttachment.ColorAttachment0);

        depth_tex = new Texture2D(
            width,
            height,
            InternalFormat.DepthComponent,
            PixelFormat.DepthComponent,
            PixelType.UnsignedByte);

        depth_tex.FrameBufferTexture(FramebufferAttachment.DepthAttachment);

        fbo.CheckStatus();
    }

    public void Bind()
    {
        fbo.Bind();
    }

    public void Dispose()
    {
        fbo.Dispose();
        color_tex.Dispose();
        depth_tex.Dispose();
    }
}
