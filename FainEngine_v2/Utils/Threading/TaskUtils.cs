namespace FainEngine_v2.Utils.Threading;

public static class TaskUtils
{
    public static async Task WhenConditionAsync(Func<bool> condition, int checkIntervalMs = 100, CancellationToken cancellationToken = default)
    {
        while (!condition())
        {
            await Task.Delay(checkIntervalMs, cancellationToken);
        }
    }
}