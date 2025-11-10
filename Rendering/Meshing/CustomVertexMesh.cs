using FainEngine_v2.Rendering.BoundingShapes;
using System.Numerics;

namespace FainEngine_v2.Rendering.Meshing;
public class CustomVertexMesh<TVertexType, TIndexType> : AMesh<TVertexType, TIndexType>
    where TVertexType : unmanaged
    where TIndexType : unmanaged
{
    protected override uint VertexCount => Vertices is null ? 0 : (uint)Vertices.Length;

    public TVertexType[]? Vertices { get; set; }

    #region Constructors
    public CustomVertexMesh() : base()
    {
        Bind();
        ApplyVertices();
        ApplyTriangles();
        VertexAttributes.SetVertexAttributes(_gl, VAO);
    }

    public CustomVertexMesh(TVertexType[] vertices, TIndexType[] trianges) : base()
    {
        Vertices = vertices;
        Triangles = trianges;

        Bind();
        ApplyVertices();
        ApplyTriangles();
        VertexAttributes.SetVertexAttributes(_gl, VAO);
    }
    #endregion

    public override void Clear()
    {
        Vertices = null;
        Triangles = null;

        VAO.Bind();
        VBO.SetData(Span<TVertexType>.Empty);
        EBO.SetData(Span<TIndexType>.Empty);
    }

    public void SetVertices(TVertexType[] data)
    {
        Vertices = data;
    }

    protected override void ApplyVertices()
    {
        VBO.SetData(Vertices);
    }

    public void RecalculateBounds()
    {
        if (Vertices == null || Vertices.Length == 0)
        {
            Bounds = default;
            return;
        }

        Vector3 pos = GetVertexPosition(Vertices[0]);
        Vector3 min = pos;
        Vector3 max = pos;

        for (int i = 1; i < Vertices.Length; i++)
        {
            pos = GetVertexPosition(Vertices[0]);
            min = Vector3.Min(min, pos);
            max = Vector3.Max(max, pos);
        }

        Bounds = new BoundingBox(min, max);
    }

    protected virtual Vector3 GetVertexPosition(TVertexType vertex) => Vector3.Zero;
}
