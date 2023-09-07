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
}
