using System.Numerics;

namespace FainEngine_v2.Extensions;
public static class PlaneExtentions
{
    public static Plane CreateFromPointNormal(Vector3 point, Vector3 normal)
    {
        return new Plane()
        {
            Normal = normal.Normalize(),
            D = Vector3.Dot(normal, point),
        };
    }

    public static float SignedDistance(this Plane plane, Vector3 point)
    {
        return Vector3.Dot(plane.Normal, point) - plane.D;
    }
}
