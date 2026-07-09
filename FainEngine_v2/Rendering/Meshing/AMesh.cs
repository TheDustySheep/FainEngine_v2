using FainEngine_v2.Core;
using FainEngine_v2.Rendering.BoundingShapes;
using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace FainEngine_v2.Rendering.Meshing;

public class AMesh<TVertexType, TIndexType> : GLObject, IMesh
    where TVertexType : unmanaged
    where TIndexType : unmanaged
{
    protected readonly BufferObject<TVertexType> VBO;
    protected readonly BufferObject<TIndexType> EBO;
    protected readonly VertexArrayObject<TVertexType, TIndexType> VAO;

    public int VertexCount => VBO.Count;
    public int TriangleCount => EBO.Count;

    public BoundingBox Bounds { get; set; }
    public bool ClipBounds { get; set; } = true;

    public AMesh()
    {
        VBO = new BufferObject<TVertexType>(BufferTargetARB.ArrayBuffer);
        EBO = new BufferObject<TIndexType>(BufferTargetARB.ElementArrayBuffer);

        VAO = new VertexArrayObject<TVertexType, TIndexType>(VBO, EBO);

        VertexAttributes.SetVertexAttributes(_GL, VAO);
        ThrowOnError("Setting Vertex Attributes");
    }

    public void SetData(List<TVertexType> vertices, List<TIndexType> triangles)
    {
        var verts = CollectionsMarshal.AsSpan(vertices);
        var tris = CollectionsMarshal.AsSpan(triangles);

        VBO.SetData(verts);
        EBO.SetData(tris);
        OnSetData(verts, tris);
    }
    public void SetData(TVertexType[] vertices, TIndexType[] triangles)
    {
        VBO.SetData(vertices);
        EBO.SetData(triangles);
        OnSetData(vertices, triangles);
    }

    public void SetData(Span<TVertexType> vertices, Span<TIndexType> triangles)
    {
        VBO.SetData(vertices);
        EBO.SetData(triangles);
        OnSetData(vertices, triangles);
    }

    public void SetData(ReadOnlySpan<TVertexType> vertices, ReadOnlySpan<TIndexType> triangles)
    {
        VBO.SetData(vertices);
        EBO.SetData(triangles);
        OnSetData(vertices, triangles);
    }

    protected virtual void OnSetData(ReadOnlySpan<TVertexType> vertices, ReadOnlySpan<TIndexType> triangles) { }

    public void Clear()
    {
        VBO.Clear();
        EBO.Clear();
    }

    public unsafe virtual void Draw()
    {
        if (VBO.Count == 0 || EBO.Count == 0)
            return;

        VAO.Bind();

        _GL.DrawElements(
            PrimitiveType.Triangles, 
            (uint)EBO.Count,
            DrawElementsType.UnsignedInt, 
            (void*)0
        );
        ThrowOnError("Drawing Mesh");
    }

    protected override void Release()
    {
        VAO.Dispose();
        VBO.Dispose();
        EBO.Dispose();
    }
}
