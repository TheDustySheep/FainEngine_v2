namespace FainEngine_v2.Extensions;

public static class FloatExtensions
{
    public static int FloorToInt(this float value)
    {
        return (int)MathF.Floor(value);
    }

    public static int CeilingToInt(this float value)
    {
        return (int)MathF.Ceiling(value);
    }

    public static float Remap(this float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
    }
}
