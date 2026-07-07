using System.Collections.Concurrent;

namespace FainEngine_v2.Core;
public static class GLDisposalService
{
    static readonly ConcurrentQueue<Action> _queue = new(); 

    public static void Enqueue(Action func)
    {
        _queue.Enqueue(func);
    }

    public static void ExecuteDispose()
    {
        while (_queue.TryDequeue(out Action? func))
            func.Invoke();
    }
}
