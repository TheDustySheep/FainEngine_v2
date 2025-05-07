using FainEngine_v2.Rendering.Materials;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.RenderObjects;
public sealed class RenderTexture : IFrameBuffer, IDisposable
{
    readonly GL _gl;

    public Texture2D ColorTexture { get; private set; }
    public Texture2D DepthTexture { get; private set; }

    readonly uint _fbo;

    public readonly int Height;
    public readonly int Width;

    public unsafe RenderTexture(GL gl, int width, int height)
    {
        Width = width;
        Height = height;

        _gl = gl;
        _gl.CreateFramebuffers(1, out _fbo);


        ColorTexture = new Texture2D(width, height, SizedInternalFormat.Rgba8, PixelFormat.Rgba, PixelType.UnsignedByte);
        DepthTexture = new Texture2D(width, height, SizedInternalFormat.DepthComponent32f, PixelFormat.DepthComponent, PixelType.Float);

        _gl.NamedFramebufferTexture(_fbo, FramebufferAttachment.ColorAttachment0, ColorTexture.Handle, 0);
        _gl.NamedFramebufferTexture(_fbo, FramebufferAttachment.DepthAttachment, DepthTexture.Handle, 0);

        var status = _gl.CheckNamedFramebufferStatus(_fbo, FramebufferTarget.Framebuffer);
        if (status != GLEnum.FramebufferComplete)
            throw new Exception($"Incomplete FBO: {status}");
    }

    public void Bind() => _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);

    public void Dispose()
    {
        _gl.DeleteFramebuffer(_fbo);
        ColorTexture.Dispose();
        DepthTexture.Dispose();
    }
}
