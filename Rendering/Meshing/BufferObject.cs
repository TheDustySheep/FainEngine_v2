using Silk.NET.OpenGL;
using System;

namespace FainEngine_v2.Rendering.Meshing;

public class BufferObject<TDataType> : IDisposable
    where TDataType : unmanaged
{
    private uint _handle;
    private BufferTargetARB _bufferType;
    private GL _gl;

    public unsafe BufferObject(GL gl, BufferTargetARB bufferType)
    {
        _gl = gl;
        _bufferType = bufferType;

        _handle = _gl.GenBuffer();
    }

    public unsafe void SetData(Span<TDataType> data)
    {
        Bind();
        fixed (void* d = data)
        {
            _gl.BufferData(_bufferType, (nuint)(data.Length * sizeof(TDataType)), d, BufferUsageARB.StaticDraw);
        }
    }

    public void Bind()
    {
        _gl.BindBuffer(_bufferType, _handle);
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_handle);
    }
}
