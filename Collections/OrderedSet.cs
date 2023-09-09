using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.Collections;
public class OrderedSet<T> : ICollection<T> where T : notnull
{
    private readonly IDictionary<T, LinkedListNode<T>> m_Dictionary;
    private readonly LinkedList<T> m_LinkedList;

    public OrderedSet() : this(EqualityComparer<T>.Default)
    {
    }

    public OrderedSet(IEqualityComparer<T> comparer)
    {
        m_Dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
        m_LinkedList = new LinkedList<T>();
    }

    public int Count => m_Dictionary.Count;

    public virtual bool IsReadOnly => m_Dictionary.IsReadOnly;

    void ICollection<T>.Add(T item)
    {
        AddLast(item);
    }

    public bool AddLast(T item)
    {
        if (m_Dictionary.ContainsKey(item)) 
            return false;

        var node = m_LinkedList.AddLast(item);
        m_Dictionary.Add(item, node);
        return true;
    }

    public bool AddFirst(T item)
    {
        if (m_Dictionary.ContainsKey(item))
            return false;

        var node = m_LinkedList.AddFirst(item);
        m_Dictionary.Add(item, node);
        return true;
    }

    public void AddOrUpdateLast(T item)
    {
        Remove(item);
        AddLast(item);
    }

    public void AddOrUpdateFirst(T item)
    {
        Remove(item);
        AddFirst(item);
    }

    public T? Dequeue()
    {
        LinkedListNode<T>? node = m_LinkedList.First;
        
        if (node is null)
            return default;

        T value = node.Value;
        m_Dictionary.Remove(value);
        m_LinkedList.Remove(node);

        return value;
    }

    public bool TryDequeue(out T? value)
    {
        LinkedListNode<T>? node = m_LinkedList.First;

        if (node is null)
        {
            value = default;
            return false;
        }

        value = node.Value;
        m_Dictionary.Remove(value);
        m_LinkedList.Remove(node);
        return true;
    }

    public bool Remove(T item)
    {
        if (item == null) 
            return false;

        if (!m_Dictionary.TryGetValue(item, out var node)) 
            return false;

        m_Dictionary.Remove(item);
        m_LinkedList.Remove(node);
        return true;
    }

    public void Clear()
    {
        m_LinkedList.Clear();
        m_Dictionary.Clear();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return m_LinkedList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Contains(T item)
    {
        return item != null && m_Dictionary.ContainsKey(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        m_LinkedList.CopyTo(array, arrayIndex);
    }
}
