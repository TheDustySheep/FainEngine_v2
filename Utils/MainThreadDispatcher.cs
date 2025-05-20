using System.Collections.Concurrent;

namespace FainEngine_v2.Utils;

public static class MainThreadDispatcher
{
    private static readonly ConcurrentQueue<Func<Task>> _tasks = new();
    private static readonly ConcurrentQueue<TaskCompletionSource<bool>> _completions = new();

    public static Task EnqueueAsync(Func<Task> func)
    {
        var tcs = new TaskCompletionSource<bool>();
        _tasks.Enqueue(func);
        _completions.Enqueue(tcs);
        return tcs.Task;
    }

    public static Task EnqueueAsync(Action action)
    {
        return EnqueueAsync(() =>
        {
            action.Invoke();
            return Task.CompletedTask;
        });
    }

    public static void ExecutePending()
    {
        while (_tasks.TryDequeue(out var task) && _completions.TryDequeue(out var tcs))
        {
            try
            {
                var result = task.Invoke();
                if (result.IsCompleted)
                {
                    tcs.TrySetResult(true);
                }
                else
                {
                    result.ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                            tcs.TrySetException(t.Exception!);
                        else
                            tcs.TrySetResult(true);
                    });
                }
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }
    }

    public static MainThreadAwaitable Yield()
    {
        return new MainThreadAwaitable();
    }

    public readonly struct MainThreadAwaitable
    {
        public MainThreadAwaiter GetAwaiter() => new MainThreadAwaiter();
    }

    public readonly struct MainThreadAwaiter : System.Runtime.CompilerServices.INotifyCompletion
    {
        public bool IsCompleted => false;

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            // Post continuation to main thread
            _ = EnqueueAsync(continuation);
        }
    }
}
