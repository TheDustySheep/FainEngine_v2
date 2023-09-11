using System.Numerics;

namespace FainEngine_v2.Extensions;
public static class Vector3Extensions
{
    public static Vector3 Normalized(this Vector3 vec)
    {
        return Vector3.Normalize(vec);
    }

    public static Vector3 Floor(this Vector3 vec)
    {
        return new Vector3
        (
            MathF.Floor(vec.X),
            MathF.Floor(vec.Y),
            MathF.Floor(vec.Z)
        );
    }


    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return a * (1 - t) + b * t;
    }

    public static Vector2 ToXY(this Vector3 vec)
    {
        return new Vector2(vec.X, vec.Y);
    }

    public static Vector2 ToXZ(this Vector3 vec)
    {
        return new Vector2(vec.X, vec.Z);
    }

    public static Vector2 ToYZ(this Vector3 vec)
    {
        return new Vector2(vec.Y, vec.Z);
    }
}
