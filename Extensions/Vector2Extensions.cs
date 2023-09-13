using System.Numerics;

namespace FainEngine_v2.Extensions;
public static class Vector2Extensions
{
    public static Vector2 Normalized(this Vector2 vec)
    {
        if (vec == default)
            return default;

        return Vector2.Normalize(vec);
    }

    public static Vector3 ToXY(this Vector2 vec)
    {
        return new Vector3(vec.X, vec.Y, 0f);
    }

    public static Vector3 ToXZ(this Vector2 vec)
    {
        return new Vector3(vec.X, 0f, vec.Y);
    }

    public static Vector3 ToYZ(this Vector2 vec)
    {
        return new Vector3(0, vec.X, vec.Y);
    }
}