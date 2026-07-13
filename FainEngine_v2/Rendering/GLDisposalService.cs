using FainEngine_v2.Utils;
using Silk.NET.OpenGL;
using System.Collections.Concurrent;

namespace FainEngine_v2.Rendering;
internal static class GLDisposalService
{
    private static ConcurrentQueue<(uint handle, GLObjectType method)> _instances = new();

    private static GL _GL = null!;

    public static void Init(GL gl)
    {
        _GL = gl;
    }

    public static void Collect()
    {
        while (_instances.TryDequeue(out var instance))
        {
            // Select the correct deletion method
            Action<uint> method = instance.method switch
            {
                GLObjectType.Buffer           => _GL.DeleteBuffer,
                GLObjectType.Framebuffer      => _GL.DeleteFramebuffer,
                GLObjectType.Program          => _GL.DeleteProgram,
                GLObjectType.ProgramPipeline  => _GL.DeleteProgramPipeline,
                GLObjectType.Query            => _GL.DeleteQuery,
                GLObjectType.Renderbuffer     => _GL.DeleteRenderbuffer, 
                GLObjectType.Sampler          => _GL.DeleteSampler,
                GLObjectType.Shader           => _GL.DeleteShader,
                GLObjectType.Texture          => _GL.DeleteTexture,
                GLObjectType.VertexArray      => _GL.DeleteVertexArray,
                _ => throw new NotImplementedException()
            };

            // Delete the object
            method.Invoke(instance.handle);
        }
    }

    public static void Delete(uint handle, GLObjectType method)
    {
        _instances.Enqueue((handle, method));
    }
}
