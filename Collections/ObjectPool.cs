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
}
