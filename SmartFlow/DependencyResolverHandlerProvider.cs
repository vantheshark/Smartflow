using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    internal class DependencyResolverHandlerProvider : IHandlerProvider
    {
        private readonly ConcurrentDictionary<Type, Type> _handlerTypeCache = new ConcurrentDictionary<Type, Type>();
        private readonly Type _cmdHandlerType = typeof(ICommandHandler<>);
        private readonly Type _asyncCmdHandlerType = typeof(IAsyncCommandHandler<>);
        private readonly Type _eventHandlerType = typeof(IEventHandler<>);
        private readonly Type _asyncEventHandlerType = typeof(IAsyncEventHandler<>);
        
        public IEnumerable<ICommandHandler<T>> GetCommandHandlers<T>() where T : Command
        {
            return ResolveHandlers<T, ICommandHandler<T>>(typeof(T), _cmdHandlerType);
        }

        public IEnumerable<IAsyncCommandHandler<T>> GetAsyncCommandHandlers<T>() where T : Command
        {
            return ResolveHandlers<T, IAsyncCommandHandler<T>>(typeof(T), _asyncCmdHandlerType);
        }

        public IEnumerable<IEventHandler<T>> GetEventHandlers<T>(Type eventType) where T : Event
        {
            return ResolveHandlers<T, IEventHandler<T>>(eventType, _eventHandlerType);
        }

        public IEnumerable<IAsyncEventHandler<T>> GetAsyncEventHandlers<T>(Type eventType) where T : Event
        {
            return ResolveHandlers<T, IAsyncEventHandler<T>>(eventType, _asyncEventHandlerType);
        }

        public IEnumerable<THandler> ResolveHandlers<T, THandler>(Type messageType, Type genericHandlerType) where T : IMessage
        {
            if (!_handlerTypeCache.ContainsKey(messageType))
            {
                _handlerTypeCache.TryAdd(messageType, genericHandlerType.MakeGenericType(messageType));
            }

            var type = _handlerTypeCache[messageType];
            return DependencyResolver.Current.GetServices(type).Select(h => (THandler)h);
        }

        public void RegisterCommandHandler<T>(ICommandHandler<T> handler) where T : Command
        {
        }

        public void RegisterCommandHandler<T>(IAsyncCommandHandler<T> handler) where T : Command
        {
        }

        public void RegisterEventHandler<T>(IEventHandler<T> handler) where T : Event
        {
        }

        public void RegisterEventHandler<T>(IAsyncEventHandler<T> handler) where T : Event
        {
        }
    }
}