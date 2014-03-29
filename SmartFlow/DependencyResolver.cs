using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Smartflow.Core
{
    /// <summary>
    /// A static class to provide default <see cref="IDependencyResolver"/>
    /// </summary>
    public static class DependencyResolver
    {
        private static IDependencyResolver _instance = new CacheDependencyResolver(new DefaultDependencyResolver());
        
        /// <summary>
        /// Current active Dependency Resolver
        /// </summary>
        public static IDependencyResolver Current
        {
            get { return _instance; }
        }

        /// <summary>
        /// Replace current Dependency Resolver by the provided one
        /// </summary>
        /// <param name="dependencyResolver"></param>
        public static void SetDependencyResolver(IDependencyResolver dependencyResolver)
        {
            _instance = dependencyResolver;
        }

        /// <summary>
        /// Wraps an IDependencyResolver and ensures single instance per-type.
        /// </summary>
        /// <remarks>
        /// Note it's possible for multiple threads to race and call the _resolver service multiple times.
        /// We'll pick one winner and ignore the others and still guarantee a unique instance.
        /// </remarks>
        private sealed class CacheDependencyResolver : IDependencyResolver
        {
            private readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();
            private readonly ConcurrentDictionary<Type, IEnumerable<object>> _cacheMultiple = new ConcurrentDictionary<Type, IEnumerable<object>>();

            private readonly IDependencyResolver _resolver;

            public CacheDependencyResolver(IDependencyResolver resolver)
            {
                _resolver = resolver;
            }

            public T GetService<T>() where T : class 
            {
                // Use a saved delegate to prevent per-call delegate allocation
                return _cache.GetOrAdd(typeof(T), type => _resolver.GetService<T>()) as T;
            }

            public IEnumerable<T> GetServices<T>() where T : class 
            {
                // Use a saved delegate to prevent per-call delegate allocation
                return _cacheMultiple.GetOrAdd(typeof(T), _resolver.GetServices<T>()).Cast<T>();
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                return _cacheMultiple.GetOrAdd(serviceType, _resolver.GetServices(serviceType));
            }
        }

        private class DefaultDependencyResolver : IDependencyResolver
        {
            public T GetService<T>() where T :class 
            {
                var serviceType = typeof (T);
                if (serviceType.IsInterface || serviceType.IsAbstract)
                {
                    return default(T);
                }

                try
                {
                    return Activator.CreateInstance(serviceType) as T;
                }
                catch
                {
                    return null;
                }
            }

            public IEnumerable<T> GetServices<T>() where T: class 
            {
                return Enumerable.Empty<T>();
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                if (serviceType.IsInterface || serviceType.IsAbstract)
                {
                    return new object[0];
                }

                try
                {
                    return new[] {Activator.CreateInstance(serviceType)};
                }
                catch
                {
                    return new object[0];
                }
            }
        }
    }
}