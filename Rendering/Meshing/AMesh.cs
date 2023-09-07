using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Meshing;

public abstract class AMesh<TVertexType, TIndexType> : IMesh
    where TVertexType : unmanaged
    where TIndexType : unmanaged
{
    protected abstract uint VertexCount { get; }

    protected GL _gl { get; }
    protected VertexArrayObject<TVertexType, TIndexType> VAO { get; set; }
    protected BufferObject<TVertexType> VBO { get; set; }
    protected BufferObject<TIndexType> EBO { get; set; }
    public TIndexType[]? Triangles { get; set; }

    public AMesh(GL gl)
    {
        _gl = gl;
        EBO = new BufferObject<TIndexType>(_gl, BufferTargetARB.ElementArrayBuffer);
        VBO = new BufferObject<TVertexType>(_gl, BufferTargetARB.ArrayBuffer);
        VAO = new VertexArrayObject<TVertexType, TIndexType>(_gl, VBO, EBO);
    }

    public void SetTriangles(TIndexType[] triangles)
    {
        Triangles = triangles;
    }

    public void Apply()
    {
        Bind();
        ApplyVertices();
        ApplyTriangles();
        SetVertexAttributes();
    }

    protected abstract void SetVertexAttributes();
    protected abstract void ApplyVertices();
    protected void ApplyTriangles()
    {
        EBO.SetData(Triangles);
    }

    public void Bind()
    {
        VAO.Bind();
    }

    public unsafe void Draw()
    {
        if (Triangles is null || Triangles.Length == 0 || VertexCount == 0)
            return;

        VAO.Bind();
        //_gl.DrawArrays(PrimitiveType.Triangles, 0, VertexCount);
        _gl.DrawElements(PrimitiveType.Triangles, (uint)Triangles.Length, DrawElementsType.UnsignedInt, (void*)0);
    }

    public void Dispose()
    {
        VAO.Dispose();
        VBO.Dispose();
        EBO.Dispose();
    }
}
