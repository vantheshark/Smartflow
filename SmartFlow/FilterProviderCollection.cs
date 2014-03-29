using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Smartflow.Core
{
    /// <summary>
    /// A oollection of filter provider
    /// </summary>
    public class FilterProviderCollection : Collection<IFilterProvider>
    {
        private static FilterComparer _filterComparer = new FilterComparer();
        private IFilterProvider[] _combinedItems;
        private IDependencyResolver _dependencyResolver;

        /// <summary>
        /// Initiazliae a FilterProviderCollection
        /// </summary>
        public FilterProviderCollection()
        {
        }

        /// <summary>
        /// Initiazliae a FilterProviderCollection with list of providers
        /// </summary>
        public FilterProviderCollection(IList<IFilterProvider> providers)
            : base(providers)
        {
        }

        internal FilterProviderCollection(IList<IFilterProvider> list, IDependencyResolver dependencyResolver)
            : base(list)
        {
            _dependencyResolver = dependencyResolver;
        }

        internal IFilterProvider[] CombinedItems
        {
            get
            {
                IFilterProvider[] combinedItems = _combinedItems;
                if (combinedItems == null)
                {
                    combinedItems = GetCombined(Items, _dependencyResolver);
                    _combinedItems = combinedItems;
                }
                return combinedItems;
            }
        }

        internal static TService[] GetCombined<TService>(IList<TService> items, IDependencyResolver resolver = null) where TService : class
        {           
            if (resolver == null)
            {
                resolver = DependencyResolver.Current;
            }
            IEnumerable<TService> services = resolver.GetServices<TService>();
            return services.Concat(items).ToArray();
        } 

        private static bool AllowMultiple(object filterInstance)
        {
            var filter = filterInstance as IFilter;
            if (filter == null)
            {
                return true;
            }

            return filter.AllowMultiple;
        }

        /// <summary>
        /// Get all filters by current handlerContext
        /// </summary>
        /// <param name="handlerContext"></param>
        /// <returns></returns>
        public IEnumerable<Filter> GetFilters(HandlerContext handlerContext)
        {
            if (handlerContext == null)
            {
                throw new ArgumentNullException("handlerContext");
            }
           

            IFilterProvider[] providers = CombinedItems;
            List<Filter> filters = new List<Filter>();
            for (int i = 0; i < providers.Length; i++)
            {
                IFilterProvider provider = providers[i];
                foreach (Filter filter in provider.GetFilters(handlerContext))
                {
                    filters.Add(filter);
                }
            }

            filters.Sort(_filterComparer);

            if (filters.Count > 1)
            {
                RemoveDuplicates(filters);
            }
            return filters;
        }

        private static void RemoveDuplicates(List<Filter> filters)
        {
            var visitedTypes = new HashSet<Type>();

            // Remove duplicates from the back forward
            for (int i = filters.Count - 1; i >= 0; i--)
            {
                Filter filter = filters[i];
                object filterInstance = filter.Instance;
                Type filterInstanceType = filterInstance.GetType();

                if (!visitedTypes.Contains(filterInstanceType) || AllowMultiple(filterInstance))
                {
                    visitedTypes.Add(filterInstanceType);
                }
                else
                {
                    filters.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Clear all registered providers
        /// </summary>
        protected override void ClearItems()
        {
            _combinedItems = null;
            base.ClearItems();
        }

        /// <summary>
        /// Insert a filter provider
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, IFilterProvider item)
        {
            _combinedItems = null;
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Remove provider at specific index
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            _combinedItems = null;
            base.RemoveItem(index);
        }

        /// <summary>
        /// Overwrite the filter at index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, IFilterProvider item)
        {
            _combinedItems = null;
            base.SetItem(index, item);
        }

        private class FilterComparer : IComparer<Filter>
        {
            public int Compare(Filter x, Filter y)
            {
                // Nulls always have to be less than non-nulls
                if (x == null && y == null)
                {
                    return 0;
                }
                if (x == null)
                {
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }

                // Sort first by order...

                if (x.Order < y.Order)
                {
                    return -1;
                }
                if (x.Order > y.Order)
                {
                    return 1;
                }

                // ...then by scope

                if (x.Scope < y.Scope)
                {
                    return -1;
                }
                if (x.Scope > y.Scope)
                {
                    return 1;
                }

                return 0;
            }
        }
    }
}