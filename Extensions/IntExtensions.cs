using System.Runtime.CompilerServices;

namespace FainEngine_v2.Extensions;

public static class IntExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Mod(this int value, int divisior)
    {
        int r = value % divisior;
        return r < 0 ? r + divisior : r;
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static int Mod(this int x, int m)
    //{
    //    return (x % m + m) % m;
    //}
}
