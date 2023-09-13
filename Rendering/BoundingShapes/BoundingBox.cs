using System.Numerics;

namespace FainEngine_v2.Rendering.BoundingShapes;

public struct BoundingBox
{
    public Vector3 Min;
    public Vector3 Max;

    public BoundingBox(Vector3 min, Vector3 max)
    {
        Min = Vector3.Min(min, max);
        Max = Vector3.Max(min, max);
    }

    public readonly void GetVertices(ref Span<Vector3> vertices)
    {
        vertices[0] = new Vector3(Min.X, Min.Y, Min.Z);
        vertices[1] = new Vector3(Min.X, Min.Y, Max.Z);
        vertices[2] = new Vector3(Min.X, Max.Y, Min.Z);
        vertices[3] = new Vector3(Min.X, Max.Y, Max.Z);
        vertices[4] = new Vector3(Max.X, Min.Y, Min.Z);
        vertices[5] = new Vector3(Max.X, Min.Y, Max.Z);
        vertices[6] = new Vector3(Max.X, Max.Y, Min.Z);
        vertices[7] = new Vector3(Max.X, Max.Y, Max.Z);
    }

    public BoundingBox Transform(Matrix4x4 model)
    {
        return new BoundingBox(
            Vector3.Transform(Min, model),
            Vector3.Transform(Max, model));
    }
}
