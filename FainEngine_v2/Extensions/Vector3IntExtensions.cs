using Silk.NET.Maths;

namespace FainEngine_v2.Extensions;

public static class Vector3IntExtensions
{
    public static Vector3D<int> Mod(this Vector3D<int> pos, int div)
    {
        return new Vector3D<int>(pos.X % div, pos.Y % div, pos.Z % div);
    }

    public static Vector3D<int> UnsignedMod(this Vector3D<int> pos, int div)
    {
        return new Vector3D<int>(mod(pos.X, div), mod(pos.Y, div), mod(pos.Z, div));
    }

    private static int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    public static Vector3D<int> BitShiftLeft(this Vector3D<int> a, int b)
    {
        a.X = Math.Abs(a.X) << b;
        a.Y = Math.Abs(a.Y) << b;
        a.Z = Math.Abs(a.Z) << b;
        return a;
    }

    public static Vector3D<int> BitShiftRight(this Vector3D<int> a, int b)
    {
        a.X = Math.Abs(a.X) >> b;
        a.Y = Math.Abs(a.Y) >> b;
        a.Z = Math.Abs(a.Z) >> b;
        return a;
    }

    public static Vector2D<int> ConvertToXZ(this Vector3D<int> a)
    {
        return new Vector2D<int>(a.X, a.Z);
    }
}