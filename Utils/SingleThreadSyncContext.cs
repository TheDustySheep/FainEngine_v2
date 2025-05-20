using System.Collections.Concurrent;

namespace FainEngine_v2.Utils
{
    public class SingleThreadSynchronizationContext : SynchronizationContext
    {
        private readonly BlockingCollection<(SendOrPostCallback, object?)> _queue = new();

        public SingleThreadSynchronizationContext(bool setContext=true)
        {
            if (setContext)
                SetSynchronizationContext(this);
        }

        public void RunOnCurrentThread()
        {
            foreach (var (callback, state) in _queue.GetConsumingEnumerable())
            {
                callback(state);
            }
        }

        public void Complete() => _queue.CompleteAdding();

        public override void Post(SendOrPostCallback d, object? state)
        {
            _queue.Add((d, state));
        }
    }
}
