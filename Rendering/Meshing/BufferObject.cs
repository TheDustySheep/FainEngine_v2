using Silk.NET.OpenGL;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FainEngine_v2.Rendering.Meshing;

public class BufferObject<TDataType> : IDisposable
    where TDataType : unmanaged
{
    public uint Handle => _handle;
    private readonly uint _handle;
    private readonly BufferTargetARB _bufferType;
    private readonly VertexBufferObjectUsage _bufferUsage;
    private readonly GL _gl;
    private bool _disposed;

    public BufferObject( 
        BufferTargetARB bufferType,
        VertexBufferObjectUsage bufferUsage = VertexBufferObjectUsage.StaticDraw
    )
    {
        _gl = GameGraphics.GL;
        _bufferType = bufferType;
        _bufferUsage = bufferUsage;

        _handle = _gl.CreateBuffer();
        _gl.NamedBufferData(_handle, ReadOnlySpan<TDataType>.Empty, _bufferUsage);
    }

    public void SetData(ReadOnlySpan<TDataType> data)
    {
        _gl.NamedBufferData(_handle, data, _bufferUsage);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _gl.DeleteBuffer(_handle);
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    ~BufferObject() => Dispose();
}
