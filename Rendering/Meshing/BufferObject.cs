using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Meshing;

public class BufferObject<TDataType> : IDisposable
    where TDataType : unmanaged
{
    private readonly uint _handle;
    private readonly BufferTargetARB _bufferType;
    private readonly GL _gl;

    public BufferObject(GL gl, BufferTargetARB bufferType)
    {
        _gl = gl;
        _bufferType = bufferType;

        _handle = _gl.GenBuffer();
    }

    public void SetData(ReadOnlySpan<TDataType> data)
    {
        Bind();
        _gl.BufferData(_bufferType, data, BufferUsageARB.StaticDraw);
    }

    public void Bind()
    {
        _gl.BindBuffer(_bufferType, _handle);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _gl.DeleteBuffer(_handle);
    }
}
