using System.Runtime.CompilerServices;

namespace FainEngine_v2.Utils
{
    public static class EasingFunctions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float x, float yMin, float yMax)
        {
            return yMin + (yMax - yMin) * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Remap(float x, float fromMin, float fromMax, float toMin, float toMax)
        {
            float t = (x - fromMin) / (fromMax - fromMin); // Normalize to 0–1
            return toMin + t * (toMax - toMin);                // Scale to target range
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RemapNeg1_1To0_1(float t)
        {
            return (t + 1f) * 0.5f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Remap0_1ToNeg1_1(float t)
        {
            return (t * 2f) - 1f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float x, float min, float max)
        {
            return MathF.Max(min, MathF.Min(max, x));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float x)
        {
            return MathF.Max(0f, MathF.Min(1f, x));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutCubic(float x)
        {
            return x < 0.5 ? 4 * x * x * x : 1 - MathF.Pow(-2 * x + 2, 3) / 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutQuint(float x)
        {
            return x < 0.5 ? 16 * x * x * x * x * x : 1 - MathF.Pow(-2 * x + 2, 5) / 2;

        }
    }
}
