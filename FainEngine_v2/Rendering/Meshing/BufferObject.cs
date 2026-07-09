using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Meshing;

public class BufferObject<TDataType> : GLObject where TDataType : unmanaged
{
    public readonly uint Handle;

    public int Capacity { get; private set; }
    public int Count { get; private set; }

    private readonly BufferTargetARB _bufferType;
    private readonly VertexBufferObjectUsage _bufferUsage;

    public BufferObject(
        BufferTargetARB bufferType,
        VertexBufferObjectUsage bufferUsage = VertexBufferObjectUsage.StaticDraw
    )
    {
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
            ThrowOnError("Setting Buffer Data");
        }
        else
        {
            _GL.NamedBufferSubData(
                Handle,
                IntPtr.Zero,
                data
            );
            ThrowOnError("Setting Existing Buffer Data");
        }
    }

    public void Clear()
    {
        Count = 0;
        Capacity = 0;

        _GL.NamedBufferData(
            Handle,
            ReadOnlySpan<TDataType>.Empty,
            _bufferUsage
        );
        ThrowOnError("Clearing Buffer Data");
    }

    protected override void Release()
    {
        _GL.DeleteBuffer(Handle);
        ThrowOnError("Releasing Buffer");
    }
}
