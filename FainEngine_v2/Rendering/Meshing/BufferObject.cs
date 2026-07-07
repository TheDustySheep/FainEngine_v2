using FainEngine_v2.Core;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Meshing;

public class BufferObject<TDataType> : IDisposable where TDataType : unmanaged
{
    public readonly uint Handle;

    public int Capacity { get; private set; }
    public int Count { get; private set; }

    private readonly BufferTargetARB _bufferType;
    private readonly VertexBufferObjectUsage _bufferUsage;
    private readonly GL _GL;

    public BufferObject(
        BufferTargetARB bufferType,
        VertexBufferObjectUsage bufferUsage = VertexBufferObjectUsage.StaticDraw
    )
    {
        _GL = GameGraphics.GL;
        _bufferType = bufferType;
        _bufferUsage = bufferUsage;

        Handle = _GL.CreateBuffer();

        Capacity = 0;
        Count = 0;
    }

    public void SetData(ReadOnlySpan<TDataType> data)
    {
        Count = data.Length;

        if (data.Length > Capacity)
        {
            Capacity = data.Length;

            _GL.NamedBufferData(
                Handle,
                data,
                _bufferUsage
            );
        }
        else
        {
            _GL.NamedBufferSubData(
                Handle,
                IntPtr.Zero,
                data
            );
        }
    }

    public void Clear()
    {
        Count = 0;

        _GL.NamedBufferData(
            Handle,
            ReadOnlySpan<TDataType>.Empty,
            _bufferUsage
        );
    }

    private bool _disposed;
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        GLDisposalService.Enqueue(() => _GL.DeleteBuffer(Handle));

        GC.SuppressFinalize(this);
    }

    ~BufferObject()
    {
        if (_disposed)
            return;

        _disposed = true;

        GLDisposalService.Enqueue(() => _GL.DeleteBuffer(Handle));
    }
}
