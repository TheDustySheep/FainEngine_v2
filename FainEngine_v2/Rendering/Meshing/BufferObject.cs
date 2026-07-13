using Silk.NET.OpenGL;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        if (data.Length > Capacity || 
            data.Length < Capacity * 0.5f)
        {
            _GL.NamedBufferData(Handle, data, _bufferUsage);
            Capacity = data.Length;
            ThrowOnError("Allocating Buffer");
        }

        _GL.NamedBufferSubData(Handle, IntPtr.Zero, data);
    }


    public void Clear()
    {
        Count = 0;
        //Capacity = 0;

        //_GL.NamedBufferData(
        //    Handle,
        //    ReadOnlySpan<TDataType>.Empty,
        //    _bufferUsage
        //);
        //ThrowOnError("Clearing Buffer Data");
    }

    protected override void Release()
    {
        GLDisposalService.Delete(Handle, GLObjectType.Buffer);
    }
}
