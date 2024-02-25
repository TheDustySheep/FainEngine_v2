using FainEngine_v2.Rendering.Materials;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.RenderObjects;
public class RenderTexture
{
    GL _gl;

    public Texture2D ColorTexture => color_tex;
    public Texture2D DepthTexture => depth_tex;

    Texture2D color_tex;
    Texture2D depth_tex;
    FrameBufferObject fbo;
    //RenderBufferObject rbo;

    public readonly int Height;
    public readonly int Width;

    public unsafe RenderTexture(GL gl, int width, int height)
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

        depth_tex = new Texture2D(
            gl,
            width,
            height,
            InternalFormat.DepthComponent,
            PixelFormat.DepthComponent,
            PixelType.UnsignedByte);

        depth_tex.FrameBufferTexture(FramebufferAttachment.DepthAttachment);

        //rbo = new RenderBufferObject(_gl, width, height, InternalFormat.Depth24Stencil8);
        //rbo.FrameBufferRenderBuffer(FramebufferAttachment.DepthStencilAttachment);

        fbo.CheckStatus();
    }

    public void BindFBO()
    {
        fbo.Bind();
    }
}
