using FainEngine_v2.Utils;

namespace FainEngine_v2.Core;
public static class GameTime
{
    public static float TotalTime { get; private set; } = 0f;
    public static float DeltaTime { get; private set; } = 0f;
    public static long TotalTicks { get; private set; }

    public const float FixedDeltaTime = 1f / 40f;
    private static float FixedUpdateOverdue = 0f;
    public static float LastFixedUpdate { get; private set; } = 0f;

    internal static bool TickFixedUpdate()
    {
        // If fixed update is due or overdue
        if (TotalTime >= LastFixedUpdate + FixedDeltaTime - FixedUpdateOverdue)
        {
            // How overdue is the fixed update
            FixedUpdateOverdue += TotalTime - (LastFixedUpdate + FixedDeltaTime);

            // Limit to 1 frame
            FixedUpdateOverdue = MathUtils.Clamp(-FixedDeltaTime, FixedDeltaTime, FixedUpdateOverdue);

            // Set last update to current time
            LastFixedUpdate = TotalTime;
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
