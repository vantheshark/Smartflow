using System.Collections.Generic;
using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    internal class FilterInfo<T> where T: class, IMessage
    {
        private readonly List<IFilter> _messageFilters = new List<IFilter>();
        private readonly List<IExceptionFilter> _exceptionFilters = new List<IExceptionFilter>();

        public FilterInfo()
        {
        }

        public FilterInfo(IEnumerable<Filter> filters)
        {
            // Split the cached filters list based on filter type and override scope.
            SplitFilters(filters);
        }

        public IList<IFilter> MessageFilters
        {
            get { return _messageFilters; }
        }

        public IList<IExceptionFilter> ExceptionFilters
        {
            get { return _exceptionFilters; }
        }

        private void SplitFilters(IEnumerable<Filter> filters)
        {
            foreach (var filter in filters)
            {
                var actionFilter = filter.Instance as IMessageFilter;
                if (actionFilter != null)
                {
                    _messageFilters.Add(actionFilter);
                }

                var actionFilter2 = filter.Instance as IMessageFilter<T>;
                if (actionFilter2 != null)
                {
                    _messageFilters.Add(actionFilter2);
                }

                var exceptionFilter = filter.Instance as IExceptionFilter;

                if (exceptionFilter != null)
                {
                    _exceptionFilters.Add(exceptionFilter);
                }
            }
        }
    }
}