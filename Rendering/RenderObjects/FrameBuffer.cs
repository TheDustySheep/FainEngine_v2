using FainEngine_v2.Rendering.Materials;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.RenderObjects;
public sealed class FrameBuffer : IFrameBuffer, IDisposable
{
    private readonly GL _gl;
    private readonly uint _fbo;
    private readonly uint _rbo;
    public Texture2D ColorTexture { get; }

    public readonly uint Height;
    public readonly uint Width;

    public FrameBuffer(int width, int height)
    {
        _gl = GameGraphics.GL;
        Width  = (uint)width;
        Height = (uint)height;

        _gl.CreateFramebuffers(1, out _fbo);
        _gl.CreateRenderbuffers(1, out _rbo);

        _gl.NamedRenderbufferStorage(_rbo, InternalFormat.Depth24Stencil8, Width, Height);

        ColorTexture = new Texture2D(width, height, SizedInternalFormat.Rgba8, PixelFormat.Rgba, PixelType.UnsignedByte);

        _gl.NamedFramebufferTexture(_fbo, FramebufferAttachment.ColorAttachment0, ColorTexture.Handle, 0);
        _gl.NamedFramebufferRenderbuffer(_fbo, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _rbo);

        var status = _gl.CheckNamedFramebufferStatus(_fbo, FramebufferTarget.Framebuffer);
        if (status != GLEnum.FramebufferComplete)
            throw new Exception($"Incomplete FBO: {status}");
    }


    public void Bind() => _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);

    public void Dispose()
    {
        _gl.DeleteFramebuffer(_fbo);
        _gl.DeleteRenderbuffer(_rbo);
        ColorTexture.Dispose();
    }
}