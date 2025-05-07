using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.RenderObjects;
public class FrameBufferObject : IDisposable
{
    protected uint _handle;
    protected GL _gl;

    public unsafe FrameBufferObject()
    {
        _gl = GameGraphics.GL;

        _gl.CreateFramebuffers(1, out _handle);
    }

    public void Bind()
    {
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _handle);
    }

    public void CheckStatus()
    {
        var fboStatus = _gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        if (fboStatus != GLEnum.FramebufferComplete)
        {
            Console.WriteLine($"Frame Buffer Error {fboStatus}");
        }
    }

    public void Dispose()
    {
        _gl.DeleteFramebuffer(_handle);
    }
}
