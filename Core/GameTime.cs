namespace FainEngine_v2.Core;
public class GameTime
{
    public static float TotalTime { get; private set; } = 0f;
    public static float DeltaTime { get; private set; } = 0f;
    public static long TotalTicks { get; private set; }

    public const float FixedDeltaTime = 1f / 60f;
    public static float LastFixedUpdate { get; private set; } = float.MinValue;
    internal static bool FixedUpdateDue => TotalTime - LastFixedUpdate >= FixedDeltaTime;

    public static void TickFixedUpdate()
    {
        LastFixedUpdate = TotalTime;
    }

    public static void Tick(float deltaTime)
    {
        DeltaTime = deltaTime;
        TotalTime += deltaTime;
        TotalTicks++;
    }
}
