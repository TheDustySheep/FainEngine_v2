using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering;
public abstract class GLObject : IDisposable
{
    protected GL _GL;

    public GLObject()
    {
        _GL = GameGraphics.GL;
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

    protected abstract void Release();

    public void ThrowOnError(string details)
    {
        var error = _GL.GetError();
        if (error == GLEnum.NoError)
            return;

        throw new Exception($"OpenGL Error [{error}] Details: {details}");
    }
}
