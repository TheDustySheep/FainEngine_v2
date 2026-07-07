using System.Runtime.CompilerServices;

namespace FainEngine_v2.ECS;
public class EntityStore
{
    int _nextEntity;
    readonly Dictionary<int, (Archetype arch, int index)> _locations = new();
    readonly Dictionary<string, Archetype> _archetypes = new(StringComparer.Ordinal);

    static string KeyFromTypes(Type[] types)
    {
        Array.Sort(types, (a, b) => String.CompareOrdinal(a.FullName, b.FullName));
        return string.Join("|", types.Select(t => t.FullName));
    }

    Archetype GetOrCreateArchetype(Type[] types)
    {
        if (types == null) types = Array.Empty<Type>();
        // distinct + sorted
        types = types.Distinct().ToArray();
        Array.Sort(types, (a, b) => String.CompareOrdinal(a.FullName, b.FullName));
        string key = KeyFromTypes(types);
        if (_archetypes.TryGetValue(key, out var arch)) return arch;
        arch = new Archetype(types);
        _archetypes[key] = arch;
        return arch;
    }

    // Create with params IComponent[]
    public Entity CreateEntity(params IComponent[] components)
    {
        var types = components?.Select(c => c.GetType()).ToArray() ?? Array.Empty<Type>();
        var arch = GetOrCreateArchetype(types);
        arch.EnsureCapacity(arch.Count + 1);

        int id = _nextEntity++;
        int idx = arch.Append(id);

        // copy components to columns
        for (int i = 0; i < types.Length; i++)
        {
            var t = types[i];
            var arr = arch.Columns[t];
            // set element at idx without boxing if possible by casting to correct array type
            var cast = ConvertComponentToArrayAndSet(arr, components[i], idx);
            if (!cast)
                arr.SetValue(components[i], idx);
        }

        _locations[id] = (arch, idx);
        return new Entity(id);
    }

    // Helper: try fast typed set for arrays of value types or reference arrays
    static bool ConvertComponentToArrayAndSet(Array arr, object component, int idx)
    {
        var elemType = arr.GetType().GetElementType();
        if (elemType == null) return false;
        try
        {
            // fastest path for known array types:
            if (elemType.IsValueType)
            {
                // this will unbox; Array.SetValue does similar; keep for clarity
                arr.SetValue(component, idx);
                return true;
            }
            else
            {
                // reference type array
                arr.SetValue(component, idx);
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetComponent<T>(Entity e) where T : struct, IComponent
    {
        var (arch, idx) = _locations[e.Id];
        return ref ((T[])arch.Columns[typeof(T)])[idx];
    }

    public bool HasComponent<T>(Entity e) where T : struct, IComponent
    {
        var (arch, _) = _locations[e.Id];
        return arch.Columns.ContainsKey(typeof(T));
    }

    public bool TryGetComponent<T>(Entity e, out T value) where T : struct, IComponent
    {
        var (arch, idx) = _locations[e.Id];
        if (arch.Columns.TryGetValue(typeof(T), out var arr))
        {
            value = ((T[])arr)[idx];
            return true;
        }
        value = default;
        return false;
    }

    // Set component value for existing component
    public void SetComponent<T>(Entity e, T value) where T : struct, IComponent
    {
        var (arch, idx) = _locations[e.Id];
        if (!arch.Columns.TryGetValue(typeof(T), out var arr))
            throw new InvalidOperationException("AEntity does not have component " + typeof(T).Name);
        ((T[])arr)[idx] = value;
    }

    // Add a single component type (value) -> migrate entity to new archetype
    public void AddComponent<T>(Entity e, T component) where T : struct, IComponent
    {
        var (srcArch, srcIndex) = _locations[e.Id];
        var newTypes = srcArch.Types.Concat(new[] { typeof(T) }).ToArray();
        var dstArch = GetOrCreateArchetype(newTypes);
        dstArch.EnsureCapacity(dstArch.Count + 1);

        int dstIndex = dstArch.Append(e.Id);

        // copy shared components
        foreach (var t in dstArch.Types)
        {
            if (srcArch.Columns.TryGetValue(t, out var srcArr) && dstArch.Columns.TryGetValue(t, out var dstArr))
                Array.Copy(srcArr, srcIndex, dstArr, dstIndex, 1);
        }

        // set new component
        ((T[])dstArch.Columns[typeof(T)])[dstIndex] = component;

        // remove from source archetype with swap
        int movedEntity = srcArch.RemoveAtSwapBack(srcIndex);
        if (movedEntity != -1)
            _locations[movedEntity] = (srcArch, srcIndex);

        _locations[e.Id] = (dstArch, dstIndex);
    }

    // Remove a single component type -> migrate entity to new archetype
    public void RemoveComponent<T>(Entity e) where T : struct, IComponent
    {
        var (srcArch, srcIndex) = _locations[e.Id];
        var typeToRemove = typeof(T);
        if (!srcArch.Columns.ContainsKey(typeToRemove)) return;

        var newTypes = srcArch.Types.Where(t => t != typeToRemove).ToArray();
        var dstArch = GetOrCreateArchetype(newTypes);
        dstArch.EnsureCapacity(dstArch.Count + 1);

        int dstIndex = dstArch.Append(e.Id);

        // copy all remaining components
        foreach (var t in dstArch.Types)
        {
            Array.Copy(srcArch.Columns[t], srcIndex, dstArch.Columns[t], dstIndex, 1);
        }

        int movedEntity = srcArch.RemoveAtSwapBack(srcIndex);
        if (movedEntity != -1)
            _locations[movedEntity] = (srcArch, srcIndex);

        _locations[e.Id] = (dstArch, dstIndex);
    }

    // Destroy entity
    public void Destroy(Entity e)
    {
        if (!_locations.TryGetValue(e.Id, out var loc)) return;
        var (arch, idx) = loc;
        int movedEntity = arch.RemoveAtSwapBack(idx);
        if (movedEntity != -1)
            _locations[movedEntity] = (arch, idx);
        _locations.Remove(e.Id);
    }

    // Query archetypes that contain all required types
    public IEnumerable<Archetype> Query(params Type[] required)
    {
        required = required?.Distinct().ToArray() ?? Array.Empty<Type>();
        foreach (var arch in _archetypes.Values)
        {
            bool match = true;
            foreach (var r in required)
            {
                if (!arch.Columns.ContainsKey(r))
                {
                    match = false;
                    break;
                }
            }
            if (match) yield return arch;
        }
    }
}
