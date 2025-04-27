using FainEngine_v2.Rendering.Materials;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.RenderObjects;
public sealed class FrameBuffer : IFrameBuffer, IDisposable
{
    readonly GL _gl;

    public Texture2D ColorTexture => color_tex;

    readonly Texture2D color_tex;
    readonly FrameBufferObject fbo;
    readonly RenderBufferObject rbo;

    public readonly int Height;
    public readonly int Width;

    public FrameBuffer(GL gl, int width, int height)
    {
        _gl = gl;
        Width = width;
        Height = height;

        fbo = new FrameBufferObject(_gl);

        color_tex = new Texture2D(
            gl,
            width,
            height,
            InternalFormat.Rgba,
            PixelFormat.Rgba,
            PixelType.UnsignedByte);

        color_tex.FrameBufferTexture(FramebufferAttachment.ColorAttachment0);

        rbo = new RenderBufferObject(_gl, width, height, InternalFormat.Depth24Stencil8);
        rbo.FrameBufferRenderBuffer(FramebufferAttachment.DepthStencilAttachment);

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
        rbo.Dispose();
    }
}