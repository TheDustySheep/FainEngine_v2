using FainEngine_v2.Rendering.Materials;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.RenderObjects;
public class RenderTexture
{
    GL _gl;

    public Texture2D Texture => tex;

    Texture2D tex;
    FrameBufferObject fbo;
    RenderBufferObject rbo;

    public readonly int Height;
    public readonly int Width;

    public unsafe RenderTexture(GL gl, int width, int height)
    {
        _gl = gl;
        Width = width;
        Height = height;

        fbo = new FrameBufferObject(_gl);

        tex = new Texture2D(
            gl,
            width,
            height,
            InternalFormat.Rgba,
            PixelFormat.Rgba,
            PixelType.UnsignedByte);

        tex.FrameBufferTexture(FramebufferAttachment.ColorAttachment0);

        rbo = new RenderBufferObject(_gl, width, height, InternalFormat.Depth24Stencil8);
        rbo.FrameBufferRenderBuffer(FramebufferAttachment.DepthStencilAttachment);

        fbo.CheckStatus();
    }

    public void BindFBO()
    {
        tex.Bind();
        fbo.Bind();
    }
}
