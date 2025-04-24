namespace FainEngine_v2.Utils;
public class WriteOverQueue<T>
{
    T[] _values;
    int _count;
    int _pointer;
    int _capacity;

    public int Count => _count;

    public WriteOverQueue(int capacity)
    {
        _capacity = capacity;
        _values = new T[capacity];
    }

    public void Add(T value)
    {
        if (_count < _capacity)
            _count++;

        _pointer = _pointer++ % _capacity;
        _values[_pointer] = value;
    }

    public void Clear()
    {
        _count = 0;
        _pointer = 0;
        Array.Clear(_values);
    }

    public T[] ToArray()
    {
        var span = _values.AsSpan(0, _count);
        return span.ToArray();
    }
}
