using System;
using System.Collections.Generic;
using System.Linq;
using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    /// <summary>
    /// List of all registered <see cref="IHandlerProvider"/>
    /// </summary>
    public class HandlerProviderCollection : List<IHandlerProvider>, IHandlerProvider
    {
        /// <summary>
        /// Get all handlers from all registered <see cref="IHandlerProvider"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IHandler<IMessage>> GetHandlers(Type messageType)
        {
            foreach(var handlerProvider in this)
            {
                foreach(var handler in handlerProvider.GetHandlers(messageType))
                {
                    yield return handler;
                }
            }
        }

        /// <summary>
        /// Register a handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void RegisterHandler<T>(IHandler<T> handler) where T : class, IMessage
        {
            var defaultProvider = this.FirstOrDefault(p => p is DefaultHandlerProvider) as DefaultHandlerProvider;
            if (defaultProvider == null)
            {
                defaultProvider = new DefaultHandlerProvider();
                Add(defaultProvider);
            }
            defaultProvider.RegisterHandler(handler);
        }
    }
}