namespace FainEngine_v2.Core;

public interface IGameTime
{
    float DeltaTime { get; }
    float FixedDeltaTimeActual { get; }
    long TotalTicks { get; }
    float TotalTime { get; }
}