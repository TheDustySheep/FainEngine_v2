using FainEngine_v2.Rendering.Materials;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.RenderObjects;
public sealed class RenderTexture : GLObject, IFrameBuffer, IDisposable
{
    public Texture2D ColorTexture { get; private set; }
    public Texture2D DepthTexture { get; private set; }

    readonly uint _fbo;

    public readonly int Height;
    public readonly int Width;

    public unsafe RenderTexture(int width, int height)
    {
        Width = width;
        Height = height;

        _GL.CreateFramebuffers(1, out _fbo);


        ColorTexture = new Texture2D(width, height, SizedInternalFormat.Rgba8, PixelFormat.Rgba);
        DepthTexture = new Texture2D(width, height, SizedInternalFormat.DepthComponent32f, PixelFormat.DepthComponent, PixelType.Float);

        _GL.NamedFramebufferTexture(_fbo, FramebufferAttachment.ColorAttachment0, ColorTexture.Handle, 0);
        _GL.NamedFramebufferTexture(_fbo, FramebufferAttachment.DepthAttachment, DepthTexture.Handle, 0);

        var status = _GL.CheckNamedFramebufferStatus(_fbo, FramebufferTarget.Framebuffer);
        if (status != GLEnum.FramebufferComplete)
            throw new Exception($"Incomplete FBO: {status}");
    }

    public void Bind()
    {
        _GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        ThrowOnError("Binding Render Texture");
    }

    protected override void Release()
    {
        _GL.DeleteFramebuffer(_fbo);
        ColorTexture.Dispose();
        DepthTexture.Dispose();
    }
}
