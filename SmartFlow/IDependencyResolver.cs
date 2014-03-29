using System;
using System.Collections.Generic;

namespace Smartflow.Core
{
    /// <summary>
    /// Provide dependency resolver utility for the library just like ASP.NET MVC
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Resolve service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>() where T : class;

        /// <summary>
        /// Resolve services
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetServices<T>() where T : class;

        /// <summary>
        /// Get services by type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        IEnumerable<object> GetServices(Type serviceType);
    }
}