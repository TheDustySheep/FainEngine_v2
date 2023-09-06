using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.Core;
public class GameTime
{
    public static float TotalTime { get; private set; } = 0f;
    public static float DeltaTime { get; private set; } = 0f;
    public static long TotalTicks { get; private set; }

    public static float FixedUpdate { get; private set; } = 1f / 50f;
    internal static float LastFixedUpdate { get; private set; } = float.MinValue;
    internal static bool FixedUpdateDue => TotalTime - LastFixedUpdate > FixedUpdate;

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
