namespace FainEngine_v2.Collections;

public class PriorityHashQueue<TElement, TPriority>
{
    private readonly PriorityQueue<TElement, TPriority> _queue = new();
    private readonly HashSet<TElement> _hash = new();

    public int Count => _queue.Count;

    public bool Enqueue(TElement element, TPriority priority)
    {
        // Alread added
        if (!_hash.Add(element))
            return false;

        _queue.Enqueue(element, priority);
        return true;
    }

    public bool TryDequeue(out TElement element, out TPriority priority)
    {
        while (_queue.Count > 0)
        {
            if (!_queue.TryDequeue(out element!, out priority!))
                continue;

            if (_hash.Remove(element))
                return true;
        }
        element = default!;
        priority = default!;
        return false;
    }

    public bool Remove(TElement item)
    {
        return _hash.Remove(item);
    }
}
