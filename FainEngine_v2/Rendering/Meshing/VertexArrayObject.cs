using FainEngine_v2.Core;
using Silk.NET.OpenGL;
using System.Runtime.CompilerServices;

namespace FainEngine_v2.Rendering.Meshing;

public class VertexArrayObject<TVertexType, TIndexType> : GLObject
    where TVertexType : unmanaged
    where TIndexType : unmanaged
{
    private readonly uint _handle;

    public VertexArrayObject(BufferObject<TVertexType> vbo, BufferObject<TIndexType> ebo)
    {
        _handle = _GL.CreateVertexArray();

        // Assign Vertex Buffer
        _GL.VertexArrayVertexBuffer(
            _handle,
            0,
            vbo.Handle,
            0,
            (uint)Unsafe.SizeOf<TVertexType>()
        );

        // Assign Index Buffer
        _GL.VertexArrayElementBuffer(_handle, ebo.Handle);
    }

    public void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, bool normalized, uint stride, uint offset)
    {
        switch (type)
        {
            case VertexAttribPointerType.Int:
                _GL.VertexArrayAttribIFormat(_handle, index, count, VertexAttribIType.Int, offset);
                break;
            case VertexAttribPointerType.UnsignedInt:
                _GL.VertexArrayAttribIFormat(_handle, index, count, VertexAttribIType.UnsignedInt, offset);
                break;
            case VertexAttribPointerType.Float:
                _GL.VertexArrayAttribFormat(_handle, index, count, VertexAttribType.Float, normalized, offset);
                break;
            case VertexAttribPointerType.Double:
                _GL.VertexArrayAttribFormat(_handle, index, count, VertexAttribType.Double, normalized, offset);
                break;
            default:
                throw new Exception($"Unsupported vertex type {Enum.GetName(type)}");
        }

        // Bind attribute index to buffer binding index 0
        _GL.VertexArrayAttribBinding(_handle, index, 0);

        // Enable attribute in VAO
        _GL.EnableVertexArrayAttrib(_handle, index);
    }

    public void Bind()
    {
        _GL.BindVertexArray(_handle);
        ThrowOnError("VAO Bind");
    }

    protected override void Release()
    {
        _GL.DeleteVertexArray(_handle);
    }
}
