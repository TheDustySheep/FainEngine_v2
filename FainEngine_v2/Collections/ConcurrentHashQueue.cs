using System.Collections.Concurrent;

namespace FainEngine_v2.Collections
{
    public class ConcurrentHashQueue<T>
    {
        private readonly ConcurrentDictionary<T, byte> _dictionary = new ConcurrentDictionary<T, byte>();

        /// <summary>
        /// Attempts to enqueue the item. Returns true if the item was added, false if it was already present.
        /// </summary>
        public bool Enqueue(T item)
        {
            return _dictionary.TryAdd(item, 0);
        }

        /// <summary>
        /// Attempts to dequeue an item. Returns true if an item was removed; false if the collection was empty.
        /// </summary>
        public bool TryDequeue(out T item)
        {
            foreach (var key in _dictionary.Keys)
            {
                if (_dictionary.TryRemove(key, out _))
                {
                    item = key;
                    return true;
                }
            }

            item = default!;
            return false;
        }

        /// <summary>
        /// Checks if the item is present in the collection.
        /// </summary>
        public bool Contains(T item)
        {
            return _dictionary.ContainsKey(item);
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public int Count => _dictionary.Count;

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <summary>
        /// Returns a snapshot of the items in the collection.
        /// </summary>
        public IEnumerable<T> Items => _dictionary.Keys;
    }
}
