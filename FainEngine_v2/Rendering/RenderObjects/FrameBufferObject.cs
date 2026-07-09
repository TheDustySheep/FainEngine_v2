using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.RenderObjects;
public class FrameBufferObject : GLObject, IDisposable
{
    protected uint _handle;

    public unsafe FrameBufferObject()
    {
        _GL.CreateFramebuffers(1, out _handle);
    }

    public void Bind()
    {
        _GL.BindFramebuffer(FramebufferTarget.Framebuffer, _handle);
    }

    public void CheckStatus()
    {
        var fboStatus = _GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        if (fboStatus != GLEnum.FramebufferComplete)
        {
            Console.WriteLine($"Frame Buffer Error {fboStatus}");
        }
    }

    protected override void Release()
    {
        _GL.DeleteFramebuffer(_handle);
    }
}
