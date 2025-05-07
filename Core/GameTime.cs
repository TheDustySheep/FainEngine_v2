namespace FainEngine_v2.Core;
public static class GameTime
{
    public static float TotalTime { get; private set; } = 0f;
    public static float DeltaTime { get; private set; } = 0f;
    public static long TotalTicks { get; private set; }

    public static  float FixedDeltaTimeActual { get; private set; } = FixedDeltaTime;
    public const   float FixedDeltaTime = 1f / 20f;

    private static float _lastFixedUpdate = 0f;
    private static float _fixedUpdateTotalTime = 0f;

    internal static bool TickFixedUpdate()
    {
        if (_fixedUpdateTotalTime < TotalTime)
        {
            if (TotalTime != _lastFixedUpdate)
            {
                FixedDeltaTimeActual = TotalTime - _lastFixedUpdate;
                _lastFixedUpdate = TotalTime;
            }

            _fixedUpdateTotalTime += FixedDeltaTime;
            return true;
        }
        else
        {

            return false;
        }
    }

    internal static void Tick(float deltaTime)
    {
        DeltaTime = deltaTime;
        TotalTime += deltaTime;
        TotalTicks++;
    }
}
