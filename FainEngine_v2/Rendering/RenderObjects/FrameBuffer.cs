using FainEngine_v2.Rendering.Materials;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.RenderObjects;
public sealed class FrameBuffer : GLObject, IFrameBuffer, IDisposable
{
    private readonly GL _GL;
    private readonly uint _fbo;
    private readonly uint _rbo;
    public Texture2D ColorTexture { get; }

    public readonly uint Height;
    public readonly uint Width;

    public FrameBuffer(int width, int height)
    {
        Width = (uint)width;
        Height = (uint)height;

        _GL.CreateFramebuffers(1, out _fbo);
        _GL.CreateRenderbuffers(1, out _rbo);

        _GL.NamedRenderbufferStorage(_rbo, InternalFormat.Depth24Stencil8, Width, Height);

        ColorTexture = new Texture2D(width, height, SizedInternalFormat.Rgba8, PixelFormat.Rgba);

        _GL.NamedFramebufferTexture(_fbo, FramebufferAttachment.ColorAttachment0, ColorTexture.Handle, 0);
        _GL.NamedFramebufferRenderbuffer(_fbo, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _rbo);

        var status = _GL.CheckNamedFramebufferStatus(_fbo, FramebufferTarget.Framebuffer);
        if (status != GLEnum.FramebufferComplete)
            throw new Exception($"Incomplete FBO: {status}");
    }


    public void Bind() => _GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);

    protected override void Release()
    {
        _GL.DeleteFramebuffer(_fbo);
        _GL.DeleteRenderbuffer(_rbo);
        ColorTexture.Dispose();
    }
}