using System.Collections.Concurrent;

namespace FainEngine_v2.Utils.Threading;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class MainThreadDispatcher
{
    // Internal queue of work to be done on the main thread
    private static readonly ConcurrentQueue<Func<Task>> _taskQueue = new();
    private static readonly List<Task> _activeTasks = new();

    /// <summary>
    /// Should be called in the engine's main update loop.
    /// Executes all queued tasks on the main thread.
    /// </summary>
    public static void ExecutePending()
    {
        while (_taskQueue.TryDequeue(out var taskFunc))
        {
            try
            {
                var task = taskFunc();
                if (!task.IsCompleted)
                    _activeTasks.Add(task);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MainThreadDispatcher task exception: {ex}");
            }
        }

        // Clean up completed tasks
        _activeTasks.RemoveAll(t => t.IsCompleted);
    }

    /// <summary>
    /// Queues an action to be executed on the main thread.
    /// </summary>
    public static void Dispatch(Action action)
    {
        _taskQueue.Enqueue(() =>
        {
            action();
            return Task.CompletedTask;
        });
    }

    /// <summary>
    /// Queues a synchronous void action to be executed on the main thread and returns a Task that completes when the action finishes.
    /// </summary>
    public static Task DispatchAsync(Action action)
    {
        var tcs = new TaskCompletionSource<object?>();
        _taskQueue.Enqueue(() =>
        {
            try
            {
                action();
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return Task.CompletedTask;
        });
        return tcs.Task;
    }

    /// <summary>
    /// Queues a function to be executed on the main thread and returns a Task with a result.
    /// </summary>
    public static Task<T> DispatchAsync<T>(Func<T> func)
    {
        var tcs = new TaskCompletionSource<T>();
        _taskQueue.Enqueue(() =>
        {
            try
            {
                var result = func();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return Task.CompletedTask;
        });
        return tcs.Task;
    }
}
