namespace FainEngine_v2.Utils;

public static class DependencyInjector
{
    private static readonly Dictionary<Type, Func<object>> _factories = new();
    private static readonly Dictionary<Type, object> _singletons = new();

    public static T RegisterSingleton<T>() where T : new()
    {
        var instance = new T();
        _singletons[typeof(T)] = instance;
        return instance;
    }

    public static T RegisterSingleton<T>(T instance) where T : notnull
    {
        _singletons[typeof(T)] = instance;
        return instance;
    }

    public static void RegisterFactory<T>() where T : new()
    {
        _factories[typeof(T)] = () => new T();
    }

    public static void RegisterFactory<T>(Func<T> factory) where T : notnull
    {
        _factories[typeof(T)] = () => factory();
    }

    public static T Resolve<T>()
    {
        var type = typeof(T);

        if (_singletons.TryGetValue(type, out var instance))
        {
            return (T)instance;
        }

        if (_factories.TryGetValue(type, out var factory))
        {
            var obj = factory();
            _singletons[type] = obj; // lazy singleton
            return (T)obj;
        }

        throw new InvalidOperationException($"Service of type {type} not registered.");
    }
}
