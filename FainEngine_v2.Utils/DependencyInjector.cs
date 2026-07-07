using System;
using System.Collections.Generic;

namespace FainEngine_v2.Utils
{
    public static class DependencyInjector
    {
        private static readonly Dictionary<Type, Func<object>> _registrations = new();
        private static readonly Dictionary<Type, object> _singletons = new();

        /// <summary>
        /// Register a singleton by type
        /// </summary>
        public static TService RegisterSingleton<TService, TImplementation>()
            where TImplementation : TService, new()
        {
            _registrations[typeof(TService)] = () =>
            {
                if (!_singletons.ContainsKey(typeof(TService)))
                {
                    _singletons[typeof(TService)] = new TImplementation();
                }
                return _singletons[typeof(TService)];
            };

            return (TService)_registrations[typeof(TService)]();
        }

        /// <summary>
        /// Register a singleton by factory
        /// </summary>
        public static TService RegisterSingleton<TService>(Func<TService> factory)
        {
            _registrations[typeof(TService)] = () =>
            {
                if (!_singletons.ContainsKey(typeof(TService)))
                {
                    _singletons[typeof(TService)] = factory.Invoke()!;
                }
                return _singletons[typeof(TService)];
            };

            return (TService)_registrations[typeof(TService)]();
        }

        /// <summary>
        /// Register a singleton by factory
        /// </summary>
        public static TService RegisterSingleton<TService>(TService service)
        {
            _registrations[typeof(TService)] = () =>
            {
                if (!_singletons.ContainsKey(typeof(TService)))
                {
                    _singletons[typeof(TService)] = service!;
                }
                return _singletons[typeof(TService)];
            };

            return (TService)_registrations[typeof(TService)]();
        }

        /// <summary>
        /// Register a transient (new instance each time) by type
        /// </summary>
        public static TService RegisterTransient<TService, TImplementation>()
            where TImplementation : TService, new()
        {
            _registrations[typeof(TService)] = () => new TImplementation();
            return (TService)_registrations[typeof(TService)]();
        }

        /// <summary>
        /// Register a transient by factory
        /// </summary>
        public static TService RegisterTransient<TService>(Func<TService> factory)
        {
            _registrations[typeof(TService)] = () => factory.Invoke()!;
            return (TService)_registrations[typeof(TService)]();
        }

        /// <summary>
        /// Resolve a dependency
        /// </summary>
        public static TService Resolve<TService>()
        {
            if (_registrations.TryGetValue(typeof(TService), out var creator))
            {
                return (TService)creator();
            }
            throw new InvalidOperationException($"Service {typeof(TService)} not registered");
        }
    }
}
