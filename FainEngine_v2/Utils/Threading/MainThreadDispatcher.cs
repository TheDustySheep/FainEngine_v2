using System.Collections.Concurrent;

namespace FainEngine_v2.Utils.Threading;

public static class MainThreadDispatcher
{
    private static readonly ConcurrentQueue<Action> _queue = new();

    public static void Enqueue(Action action)
    {
        _queue.Enqueue(action);
    }

    internal static void Execute()
    {
        // Limit dispatch count per frame - prevents issues if background threads outpace the main thread
        int count = _queue.Count;

        for (int i = 0; i < count; i++)
        {
            if (!_queue.TryDequeue(out var action))
                return;

            action.Invoke();
        }
    }
}
