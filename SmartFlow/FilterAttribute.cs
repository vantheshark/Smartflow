using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Smartflow.Core
{
    /// <summary>
    /// A base filter class which is very similar to Asp.NET MVC FilterAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public abstract class FilterAttribute : Attribute, IFilter
    {
        private static readonly ConcurrentDictionary<Type, bool> _multiuseAttributeCache = new ConcurrentDictionary<Type, bool>();
        private int _order = Filter.DefaultOrder;

        /// <summary>
        /// Determine whether the filter can be declared multiple time
        /// </summary>
        public bool AllowMultiple
        {
            get { return AllowsMultiple(GetType()); }
        }

        /// <summary>
        /// The order which the filter will be executed
        /// </summary>
        public int Order
        {
            get { return _order; }
            set
            {
                if (value < Filter.DefaultOrder)
                {
                    throw new ArgumentOutOfRangeException("value", "Order must be greater than or equal 0");
                }
                _order = value;
            }
        }

        private static bool AllowsMultiple(Type attributeType)
        {
            return _multiuseAttributeCache.GetOrAdd(attributeType, type => type.GetCustomAttributes(typeof(AttributeUsageAttribute), true)
                                                                               .Cast<AttributeUsageAttribute>()
                                                                               .First()
                                                                               .AllowMultiple);
        }
    }
}