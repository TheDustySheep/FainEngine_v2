namespace FainEngine_v2.Extensions;

public static class IntExtensions
{
    public static int Mod(this int value, int divisior)
    {
        int r = value % divisior;
        return r < 0 ? r + divisior : r;
    }
}
