using System.Runtime.CompilerServices;

namespace FainEngine_v2.ECS;
public sealed class Archetype
{
    public readonly Type[] Types;                 // sorted, distinct
    public readonly Dictionary<Type, Array> Columns;
    public int Count;
    public int[] Entities;
    const int DefaultCapacity = 64;

    public Archetype(Type[] types)
    {
        Types = types;
        Columns = new Dictionary<Type, Array>(types.Length);
        Entities = new int[DefaultCapacity];
        foreach (var t in types)
            Columns[t] = Array.CreateInstance(t, DefaultCapacity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int required)
    {
        if (Entities.Length >= required) return;
        int newCap = Math.Max(required, Entities.Length * 2);
        Array.Resize(ref Entities, newCap);

        var keys = new List<Type>(Columns.Keys);
        foreach (var t in keys)
        {
            var arr = Columns[t];
            if (arr.Length >= newCap) continue;
            var newArr = Array.CreateInstance(t, newCap);
            Array.Copy(arr, 0, newArr, 0, Count);
            Columns[t] = newArr;
        }
    }

    // Append a new row; caller must ensure capacity.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Append(int entityId)
    {
        int idx = Count++;
        Entities[idx] = entityId;
        return idx;
    }

    // Copy one row from srcIndex into this at dstIndex for the matching Types array
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyFrom(Archetype src, int srcIndex, int dstIndex)
    {
        // assume dst and src share component types where appropriate
        foreach (var t in Types)
        {
            if (src.Columns.TryGetValue(t, out var srcArr) && Columns.TryGetValue(t, out var dstArr))
                Array.Copy(srcArr, srcIndex, dstArr, dstIndex, 1);
        }
    }

    // Copy a single component value into this archetype at index
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComponentAt(Type t, object component, int index)
    {
        ((object[])Columns[t])[index] = component; // will throw if wrong, but faster cast not possible generically
    }

    // Remove row at index by swap-back. Returns movedEntityId or -1 if none moved.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int RemoveAtSwapBack(int index)
    {
        int lastIndex = Count - 1;
        Count--;
        if (index == lastIndex)
        {
            // no move necessary
            Entities[lastIndex] = 0;
            // Clear components slot optional (not necessary)
            return -1;
        }

        int movedEntity = Entities[lastIndex];
        Entities[index] = movedEntity;

        foreach (var kv in Columns)
        {
            var arr = kv.Value;
            Array.Copy(arr, lastIndex, arr, index, 1);
            // optional clear last slot:
            // arr.SetValue(null, lastIndex);
        }

        Entities[lastIndex] = 0;
        return movedEntity;
    }
}
