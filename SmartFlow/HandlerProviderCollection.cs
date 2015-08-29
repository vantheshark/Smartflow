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
        /// Get all command handlers from all registered <see cref="IHandlerProvider"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICommandHandler<T>> GetCommandHandlers<T>() where T : Command
        {
            return this.SelectMany(handlerProvider => handlerProvider.GetCommandHandlers<T>());
        }

        /// <summary>
        /// Get all async command handlers from all registered <see cref="IHandlerProvider"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IAsyncCommandHandler<T>> GetAsyncCommandHandlers<T>() where T : Command
        {
            return this.SelectMany(handlerProvider => handlerProvider.GetAsyncCommandHandlers<T>());
        }

        /// <summary>
        /// Get all event handlers from all registered <see cref="IHandlerProvider"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEventHandler<T>> GetEventHandlers<T>(Type eventType) where T : Event
        {
            return this.SelectMany(handlerProvider => handlerProvider.GetEventHandlers<T>(eventType));
        }

        /// <summary>
        /// Get all async event handlers from all registered <see cref="IHandlerProvider"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IAsyncEventHandler<T>> GetAsyncEventHandlers<T>(Type eventType) where T : Event
        {
            return this.SelectMany(handlerProvider => handlerProvider.GetAsyncEventHandlers<T>(eventType));
        }

        /// <summary>
        /// Register a command handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void RegisterCommandHandler<T>(ICommandHandler<T> handler) where T : Command
        {
            var defaultProvider = DefaultHandlerProvider();
            defaultProvider.RegisterCommandHandler(handler);
        }

        /// <summary>
        /// Register an async command handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void RegisterCommandHandler<T>(IAsyncCommandHandler<T> handler) where T : Command
        {
            var defaultProvider = DefaultHandlerProvider();
            defaultProvider.RegisterCommandHandler(handler);
        }

        /// <summary>
        /// Register an event handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void RegisterEventHandler<T>(IEventHandler<T> handler) where T : Event
        {
            var defaultProvider = DefaultHandlerProvider();
            defaultProvider.RegisterEventHandler(handler);
        }

        /// <summary>
        /// Register an async event handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void RegisterEventHandler<T>(IAsyncEventHandler<T> handler) where T : Event
        {
            var defaultProvider = DefaultHandlerProvider();
            defaultProvider.RegisterEventHandler(handler);
        }

        private DefaultHandlerProvider DefaultHandlerProvider()
        {
            var defaultProvider = this.FirstOrDefault(p => p is DefaultHandlerProvider) as DefaultHandlerProvider;
            if (defaultProvider == null)
            {
                defaultProvider = new DefaultHandlerProvider();
                Add(defaultProvider);
            }
            return defaultProvider;
        }
    }
}