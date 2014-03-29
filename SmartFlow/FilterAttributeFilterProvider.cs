using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Smartflow.Core
{
    /// <summary>
    /// An implementation of <see cref="IFilterProvider"/> that get Filters from HandlerContext
    /// </summary>
    public class FilterAttributeFilterProvider : IFilterProvider
    {
        /// <summary>
        /// Get all filters for the current HandlerContext
        /// </summary>
        /// <param name="handlerContext"></param>
        /// <returns></returns>
        public virtual IEnumerable<Filter> GetFilters(HandlerContext handlerContext)
        {
            if (handlerContext != null)
            {
                foreach (FilterAttribute attr in GetHandlerAttributes(handlerContext))
                {
                    yield return new Filter(attr, FilterScope.Handler,  order: null);
                }

                foreach (FilterAttribute attr in GetHandlerMethodAttributes(handlerContext))
                {
                    yield return new Filter(attr, FilterScope.Action, order: null);
                }

                foreach (FilterAttribute attr in GetMessageAttributes(handlerContext))
                {
                    yield return new Filter(attr, FilterScope.Message, order: null);
                }
            }   
        }

        /// <summary>
        /// Get attributes decorated on the method handle of the handler
        /// </summary>
        /// <param name="handlerContext"></param>
        /// <returns></returns>
        protected virtual IEnumerable<FilterAttribute> GetHandlerAttributes(HandlerContext handlerContext)
        {
            MethodInfo[] allMethods = handlerContext.HandlerType.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public);
            var mInfo = allMethods.FirstOrDefault(x => x.Name == "Handle" && 
                                                  x.GetParameters().Length == 1 && 
                                                  x.GetParameters()[0].ParameterType == handlerContext.MessageType);
            return mInfo != null
                         ? (IEnumerable<FilterAttribute>) ReflectedAttributeCache.GetMethodFilterAttributes(mInfo)
                         : new FilterAttribute[0];
        }

        /// <summary>
        /// Get attributes decorated on the handler class
        /// </summary>
        /// <param name="handlerContext"></param>
        /// <returns></returns>
        protected virtual IEnumerable<FilterAttribute> GetHandlerMethodAttributes(HandlerContext handlerContext)
        {
            return ReflectedAttributeCache.GetTypeFilterAttributes(handlerContext.HandlerType);
        }

        /// <summary>
        ///  Get attributes decorated on the message
        /// </summary>
        /// <param name="handlerContext"></param>
        /// <returns></returns>
        protected virtual IEnumerable<FilterAttribute> GetMessageAttributes(HandlerContext handlerContext)
        {
            return ReflectedAttributeCache.GetTypeFilterAttributes(handlerContext.MessageType);
        }
    }
}