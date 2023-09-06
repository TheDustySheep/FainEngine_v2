using System.Collections.Concurrent;

namespace FainEngine_v2.Collections;
public class ConcurrentHashSet<T> where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> dict = new();

    public bool Contains(T item)
    {
        return dict.ContainsKey(item);
    }

    public bool Add(T item)
    {
        return dict.TryAdd(item, 0);
    }

    public bool Remove(T item)
    {
        return dict.TryRemove(item, out var _);
    }

    public void Clear()
    {
        dict.Clear();
    }
}
