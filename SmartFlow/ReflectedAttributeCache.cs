using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Smartflow.Core
{
    /// <summary>
    /// Copy from Asp.NET MVC
    /// </summary>
    internal static class ReflectedAttributeCache
    {
        private static readonly ConcurrentDictionary<MethodInfo, ReadOnlyCollection<FilterAttribute>> _methodFilterAttributeCache = new ConcurrentDictionary<MethodInfo, ReadOnlyCollection<FilterAttribute>>();

        private static readonly ConcurrentDictionary<Type, ReadOnlyCollection<FilterAttribute>> _typeFilterAttributeCache = new ConcurrentDictionary<Type, ReadOnlyCollection<FilterAttribute>>();

        public static ReadOnlyCollection<FilterAttribute> GetTypeFilterAttributes(Type type)
        {
            return GetAttributes(_typeFilterAttributeCache, type);
        }

        public static ReadOnlyCollection<FilterAttribute> GetMethodFilterAttributes(MethodInfo methodInfo)
        {
            return GetAttributes(_methodFilterAttributeCache, methodInfo);
        }

        private static ReadOnlyCollection<TAttribute> GetAttributes<TMemberInfo, TAttribute>(ConcurrentDictionary<TMemberInfo, ReadOnlyCollection<TAttribute>> lookup, TMemberInfo memberInfo)
            where TAttribute : Attribute
            where TMemberInfo : MemberInfo
        {
            // Frequently called, so use a static delegate
            // An inline delegate cannot be used because the C# compiler does not cache inline delegates that reference generic method arguments
            return lookup.GetOrAdd(memberInfo, CachedDelegates<TMemberInfo, TAttribute>.GetCustomAttributes);
        }

        private static class CachedDelegates<TMemberInfo, TAttribute>
            where TAttribute : Attribute
            where TMemberInfo : MemberInfo
        {
            internal static readonly Func<TMemberInfo, ReadOnlyCollection<TAttribute>> GetCustomAttributes = memberInfo =>
            {
                return new ReadOnlyCollection<TAttribute>((TAttribute[])memberInfo.GetCustomAttributes(typeof(TAttribute), true));
            };
        }
    }
}