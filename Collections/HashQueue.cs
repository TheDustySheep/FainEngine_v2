using System.Collections;

namespace FainEngine_v2.Collections;
public class HashQueue<T> : IEnumerable<T>
{
    readonly HashSet<T> hash = new();
    Queue<T> queue = new();

    public int Count => hash.Count;

    public void Enqueue(T item)
    {
        if (hash.Contains(item))
            return;

        hash.Add(item);
        queue.Enqueue(item);
    }

    public bool TryDequeue(out T value)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        if (queue.TryDequeue(out value))
        {
            hash.Remove(value);
            return true;
        }
        else
            return false;
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    public T Dequeue()
    {
        var item = queue.Dequeue();
        hash.Remove(item);
        return item;
    }

    public void Clear()
    {
        hash.Clear();
        queue.Clear();
    }

    public bool Contains(T item)
    {
        return hash.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        queue.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        if (item is null)
            return false;

        if (hash.Remove(item))
        {
            queue = new Queue<T>(queue.Where(s => s is null || !s.Equals(item)));
            return true;
        }

        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return queue.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return queue.GetEnumerator();
    }
}
