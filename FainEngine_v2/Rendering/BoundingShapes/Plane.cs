using FainEngine_v2.Extensions;
using System.Numerics;

namespace FainEngine_v2.Rendering.BoundingShapes;

public struct Plane
{
    public Vector3 Normal;
    public float Distance;

    public Plane(Vector3 normal, float distance) : this()
    {
        Normal = normal.Normalized();
        Distance = distance;
    }

    public Plane(Vector3 point, Vector3 normal)
    {
        Normal = normal.Normalized();
        Distance = Vector3.Dot(Normal, point);
    }

    public float HalfSpaceTest(Vector3 point)
    {
        return Vector3.Dot(point, Normal) - Distance;
    }
    public bool IsAbove(Vector3 point)
    {
        return Vector3.Dot(point, Normal) >= Distance;
    }
}
