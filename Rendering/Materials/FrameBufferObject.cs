using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.Rendering.Materials;
public class FrameBufferObject
{
    protected uint _handle;
    protected GL _gl;

    public unsafe FrameBufferObject(GL gl)
    {
        _gl = gl;
        _handle = _gl.GenFramebuffer();
    }

    public void Bind()
    {
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _handle);
    }

    public void CheckStatus()
    {
        if (_gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == GLEnum.FramebufferComplete)
        {
            Console.WriteLine("Yay framebuffer made successfully");
        }
        else
        {
            Console.WriteLine("No frame buffer :(");
        }
    }
}
