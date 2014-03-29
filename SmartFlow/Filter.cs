using System;

namespace Smartflow.Core
{
    /// <summary>
    /// Copy from System.Web.Mvc, it's just a wrapper around IFilter or any custom filter
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// The default order
        /// </summary>
        public const int DefaultOrder = -1;

        /// <summary>
        /// Initizalie a filter by the instance of the filter and its order
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="filterScope"> </param>
        /// <param name="order"></param>
        public Filter(object instance, FilterScope filterScope, int? order)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (order == null)
            {
                var mvcFilter = instance as IFilter;
                if (mvcFilter != null)
                {
                    order = mvcFilter.Order;
                }
            }

            Instance = instance;
            Order = order ?? DefaultOrder;
            Scope = filterScope;
        }

        /// <summary>
        /// The instance of the filter object
        /// </summary>
        public object Instance { get; protected set; }

        /// <summary>
        /// The order of the filter
        /// </summary>
        public int Order { get; protected set; }

        /// <summary>
        /// The scope of the filter
        /// </summary>
        public FilterScope Scope { get; protected set; }
    }
}