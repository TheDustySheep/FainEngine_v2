using System.Collections.Concurrent;

namespace FainEngine_v2.Collections
{
    public class ObjectPool<T> where T : new()
    {
        public int MaxItems { get; set; } = int.MaxValue;
        public readonly ConcurrentBag<T> Bag = new();

        public T Request()
        {
            if (Bag.TryTake(out var item))
                return item;

            return new T();
        }

        public void Return(T item)
        {
            if (Bag.Count >= MaxItems)
                return;

            Bag.Add(item);
        }
    }

    public class LimitedObjectPoolFactory<T>
    {
        private readonly int _maxItems;
        public readonly ConcurrentBag<T> Bag = new();
        public LimitedObjectPoolFactory(int count, Func<T> factory)
        {
            _maxItems = count;
            for (int i = 0; i < count; i++)
                Bag.Add(factory.Invoke());
        }

        public bool TryRequest(out T item)
        {
            return Bag.TryTake(out item!);
        }

        public void Return(T item)
        {
            if (Bag.Count >= _maxItems)
                return;

            Bag.Add(item);
        }
    }

    public class LimitedObjectPool<T> where T : new()
    {
        private readonly int _maxItems;
        public readonly ConcurrentBag<T> Bag = new();
        public LimitedObjectPool(int count)
        {
            _maxItems = count;
            for (int i = 0; i < count; i++)
                Bag.Add(new T());
        }

        public bool TryRequest(out T item)
        {
            return Bag.TryTake(out item!);
        }

        public void Return(T item)
        {
            if (Bag.Count >= _maxItems)
                return;

            Bag.Add(item);
        }
    }

    public class ObjectPoolFactory<T>
    {
        public int MaxItems { get; set; } = int.MaxValue;
        public readonly ConcurrentBag<T> Bag = new();
        private readonly Func<T> _factory;

        public ObjectPoolFactory(Func<T> factory)
        {
            _factory = factory;
        }

        public T Request()
        {
            if (Bag.TryTake(out var item))
                return item;

            return _factory.Invoke();
        }

        public void Return(T item)
        {
            if (Bag.Count >= MaxItems)
                return;

            Bag.Add(item);
        }
    }

    public class LimitedAsyncObjectPool<T>
    {
        private readonly SemaphoreSlim _limiter;
        private readonly ConcurrentQueue<T> _queue = new();

        public LimitedAsyncObjectPool(int limit, Func<T> factory)
        {
            _limiter = new(limit);
            for (int i = 0; i < limit; i++)
                _queue.Enqueue(factory.Invoke());
        }

        public async Task<T> Request()
        {
            await _limiter.WaitAsync();

            if (_queue.TryDequeue(out var item))
                return item;

            throw new Exception("Unable to provide pooled item. Requests exceeded capacity");
        }

        public void Return(T item)
        {
            _queue.Enqueue(item);
            _limiter.Release();
        }
    }
}
