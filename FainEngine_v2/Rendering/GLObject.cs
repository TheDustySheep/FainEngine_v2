using FainEngine_v2.Utils;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering;
public abstract class GLObject : IDisposable
{
    protected GL _GL;

    public GLObject()
    {
        _GL = DependencyInjector.Resolve<GL>();
    }

    private bool _disposed;
    public void Dispose()
    {
        if (_disposed) 
            return;
        _disposed = true;

        Release();
        GC.SuppressFinalize(this);
    }

    ~GLObject() => Dispose();

    protected abstract void Release();

    public void ThrowOnError(string details)
    {
        var error = _GL.GetError();
        if (error == GLEnum.NoError)
            return;

        throw new Exception($"OpenGL Error [{error}] Details: {details}");
    }
}

public enum GLObjectType
{
    Buffer,
    Framebuffer,
    Program,
    ProgramPipeline,
    Query,
    Renderbuffer,
    Sampler,
    Shader,
    Texture,
    VertexArray
}