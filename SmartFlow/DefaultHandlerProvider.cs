using System;
using System.Collections.Generic;
using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    internal class DefaultHandlerProvider : IHandlerProvider
    {
        private static readonly Dictionary<Type, List<IHandler<IMessage>>> AllHandlers = new Dictionary<Type, List<IHandler<IMessage>>>();

        public IEnumerable<IHandler<IMessage>> GetHandlers(Type messageType)
        {
            List<IHandler<IMessage>> handlers;
            if (AllHandlers.TryGetValue(messageType, out handlers))
            {
                return handlers;
            }
            return new IHandler<IMessage>[0];
        }

        /// <summary>
        /// Register a handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void RegisterHandler<T>(IHandler<T> handler) where T : class, IMessage
        {
            List<IHandler<IMessage>> handlers;
            if (!AllHandlers.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<IHandler<IMessage>>();
                AllHandlers.Add(typeof(T), handlers);
            }

            handlers.Add(new HandlerWrapper<T>(handler));
        }
    }
}