using Silk.NET.OpenGL;
using System.Runtime.CompilerServices;

namespace FainEngine_v2.Rendering.Meshing;

public class VertexArrayObject<TVertexType, TIndexType> : IDisposable
    where TVertexType : unmanaged
    where TIndexType : unmanaged
{
    private readonly uint _handle;
    private readonly GL _gl;

    public VertexArrayObject(GL gl, BufferObject<TVertexType> vbo, BufferObject<TIndexType> ebo)
    {
        _gl = gl;

        _handle = _gl.CreateVertexArray();

        // Assign Vertex Buffer
        _gl.VertexArrayVertexBuffer(
            _handle,
            0,
            vbo.Handle,
            0,
            (uint)Unsafe.SizeOf<TVertexType>()
        );

        // Assign Index Buffer
        _gl.VertexArrayElementBuffer(_handle, ebo.Handle);
    }

    public void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, bool normalized, uint stride, uint offset)
    {
        switch (type)
        {
            case VertexAttribPointerType.Int:
                _gl.VertexArrayAttribIFormat(_handle, index, count, VertexAttribIType.Int, offset);
                break;
            case VertexAttribPointerType.UnsignedInt:
                _gl.VertexArrayAttribIFormat(_handle, index, count, VertexAttribIType.UnsignedInt, offset);
                break;
            case VertexAttribPointerType.Float:
                _gl.VertexArrayAttribFormat(_handle, index, count, VertexAttribType.Float, normalized, offset);
                break;
            case VertexAttribPointerType.Double:
                _gl.VertexArrayAttribFormat(_handle, index, count, VertexAttribType.Double, normalized, offset);
                break;
            default:
                throw new Exception($"Unsupported vertex type {Enum.GetName(type)}");
        }

        // Bind attribute index to buffer binding index 0
        _gl.VertexArrayAttribBinding(_handle, index, 0);

        // Enable attribute in VAO
        _gl.EnableVertexArrayAttrib(_handle, index);
    }

    public void Bind()
    {
        _gl.BindVertexArray(_handle);
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(_handle);
    }
}
