namespace FainEngine_v2.Core;
public static class GameTime
{
    public static float TotalTime { get; private set; } = 0f;
    public static float DeltaTime { get; private set; } = 0f;
    public static long TotalTicks { get; private set; }

    private static float FixedUpdateTotalTime = 0f;
    public const float FixedDeltaTime = 1f / 20f;

    internal static bool TickFixedUpdate()
    {
        if (FixedUpdateTotalTime < TotalTime)
        {
            FixedUpdateTotalTime += FixedDeltaTime;
            return true;
        }
        return false;
    }

    internal static void Tick(float deltaTime)
    {
        DeltaTime = deltaTime;
        TotalTime += deltaTime;
        TotalTicks++;
    }
}
