namespace FainEngine_v2.Utils;

public static class MathUtils
{
    public static float DegreesToRadians(float degrees)
    {
        return MathF.PI / 180f * degrees;
    }

    public static float Lerp(float a, float b, float t)
    {
        return a * (1 - t) + b * t;
    }

    public static float InvLerp(float a, float b, float v)
    {
        return (v - a) / (b - a);
    }

    public static float Remap(float iMin, float iMax, float oMin, float oMax, float v)
    {
        float t = InvLerp(iMin, iMax, v);
        return Lerp(oMin, oMax, t);
    }

    public static float Clamp(float min, float max, float value)
    {
        return MathF.Min(max, MathF.Max(min, value));
    }

    public static float Min(float a, float b, float c)
    {
        return MathF.Min(a, MathF.Min(b, c));
    }

    public static float Min(params float[] values)
    {
        if (values.Length == 0)
            return 0f;

        float min = values[0];

        for (int i = 1; i < values.Length; i++)
        {
            min = MathF.Min(min, values[i]);
        }

        return min;
    }

    public static float Max(float a, float b, float c)
    {
        return MathF.Max(a, MathF.Max(b, c));
    }

    public static float Max(params float[] values)
    {
        if (values.Length == 0)
            return 0f;

        float max = values[0];

        for (int i = 1; i < values.Length; i++)
        {
            max = MathF.Max(max, values[i]);
        }

        return max;
    }

    public static float MoveTowards(float current, float target, float maxDelta)
    {
        if (MathF.Abs(target - current) <= maxDelta)
        {
            return target;
        }
        return current + MathF.Sign(target - current) * maxDelta;
    }
}