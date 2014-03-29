using System.Collections.Generic;

namespace Smartflow.Core
{
    /// <summary>
    /// Get all filters by HandlerContext
    /// </summary>
    public interface IFilterProvider
    {
        /// <summary>
        /// Get all filters for current HandlerContext
        /// </summary>
        /// <param name="handlerContext"></param>
        /// <returns></returns>
        IEnumerable<Filter> GetFilters(HandlerContext handlerContext);
    }
}
