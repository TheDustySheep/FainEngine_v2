using Silk.NET.OpenGL;

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

        _handle = _gl.GenVertexArray();
        Bind();
        vbo.Bind();
        ebo.Bind();
    }

    public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, bool normalized, uint stride, uint offset)
    {
        switch (type)
        {
            case VertexAttribPointerType.Int:
                _gl.VertexAttribIPointer(index, count, VertexAttribIType.Int, stride, (void*)offset);
                break;
            case VertexAttribPointerType.UnsignedInt:
                _gl.VertexAttribIPointer(index, count, VertexAttribIType.UnsignedInt, stride, (void*)offset);
                break;
            case VertexAttribPointerType.Float:
                _gl.VertexAttribPointer(index, count, type, normalized, stride, (void*)offset);
                break;
            case VertexAttribPointerType.Double:
                _gl.VertexAttribPointer(index, count, type, normalized, stride, (void*)offset);
                break;
            default:
                throw new Exception($"Unsupported vertex type {Enum.GetName(type)}");
        }

        _gl.EnableVertexAttribArray(index);
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
