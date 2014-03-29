using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Smartflow.Core
{
    /// <summary>
    /// Global filter connection
    /// </summary>
    public sealed class GlobalFilterCollection : IEnumerable<Filter>, IFilterProvider
    {
        private List<Filter> _filters = new List<Filter>();

        /// <summary>
        /// Get total global registered filters
        /// </summary>
        public int Count
        {
            get { return _filters.Count; }
        }

        /// <summary>
        /// Add a filter 
        /// </summary>
        /// <param name="filter"></param>
        public void Add(object filter)
        {
            AddInternal(filter, order: null);
        }

        /// <summary>
        /// Add a filter with custom order
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        public void Add(object filter, int order)
        {
            AddInternal(filter, order);
        }

        private void AddInternal(object filter, int? order)
        {
            ValidateFilterInstance(filter);
            _filters.Add(new Filter(filter, FilterScope.Global, order));
        }

        /// <summary>
        /// Clear all registered global filter
        /// </summary>
        public void Clear()
        {
            _filters.Clear();
        }

        /// <summary>
        /// Check if a filter has already been registered
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool Contains(object filter)
        {
            return _filters.Any(f => f.Instance == filter);
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Filter> GetEnumerator()
        {
            return _filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _filters.GetEnumerator();
        }

        IEnumerable<Filter> IFilterProvider.GetFilters(HandlerContext handlerContext)
        {
            return this;
        }

        /// <summary>
        /// Remove a registered global filter
        /// </summary>
        /// <param name="filter"></param>
        public void Remove(object filter)
        {
            _filters.RemoveAll(f => f.Instance == filter);
        }

        private static void ValidateFilterInstance(object instance)
        {
            if (instance == null || !(instance is IFilter))
            {
                throw new InvalidOperationException("instance is not a IFilter");
            }
        }
    }
}