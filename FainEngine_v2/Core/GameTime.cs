namespace FainEngine_v2.Core;
public class GameTime : IGameTime
{
    public float TotalTime { get; private set; } = 0f;
    public float DeltaTime { get; private set; } = 0f;
    public long TotalTicks { get; private set; }

    public float FixedDeltaTimeActual { get; private set; } = FixedDeltaTime;
    public const float FixedDeltaTime = 1f / 20f;

    private float _lastFixedUpdate = 0f;
    private float _fixedUpdateTotalTime = 0f;

    internal bool TickFixedUpdate()
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

    internal void Tick(float deltaTime)
    {
        DeltaTime = deltaTime;
        TotalTime += deltaTime;
        TotalTicks++;
    }
}
