using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.RenderObjects;
public class FrameBufferObject
{
    protected uint _handle;
    protected GL _gl;

    public unsafe FrameBufferObject(GL gl)
    {
        _gl = gl;
        _handle = _gl.GenFramebuffer();
        Bind();
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
}
