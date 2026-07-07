using FainEngine_v2.Rendering.BoundingShapes;
using System.Numerics;

namespace FainEngine_v2.Rendering.Meshing;
public class WorldSpaceMesh<TVertexType, TIndexType> : AMesh<TVertexType, TIndexType>
    where TVertexType : unmanaged
    where TIndexType : unmanaged
{
    Func<TVertexType, Vector3> _vertexPosition;

    public WorldSpaceMesh(Func<TVertexType, Vector3> vertexPosition) : base()
    {
        _vertexPosition = vertexPosition;
    }

    protected override void OnSetData(ReadOnlySpan<TVertexType> vertices, ReadOnlySpan<TIndexType> triangles)
    {
        RecalculateBounds(vertices);
    }

    public void RecalculateBounds(ReadOnlySpan<TVertexType> vertices)
    {
        if (vertices.Length == 0)
        {
            Bounds = default;
            return;
        }

        Vector3 min = Vector3.Zero;
        Vector3 max = Vector3.Zero;

        for (int i = 1; i < vertices.Length; i++)
        {
            Vector3 pos = _vertexPosition.Invoke(vertices[0]);
            min = Vector3.Min(min, pos);
            max = Vector3.Max(max, pos);
        }

        Bounds = new BoundingBox(min, max);
    }
}
