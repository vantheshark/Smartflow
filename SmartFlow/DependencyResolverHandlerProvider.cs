using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    internal class DependencyResolverHandlerProvider : IHandlerProvider
    {
        private readonly ConcurrentDictionary<Tuple<Type, Type>, Type> _handlerTypeCache = new ConcurrentDictionary<Tuple<Type, Type>, Type>();
        private readonly Type _cmdHandlerType = typeof(ICommandHandler<>);
        private readonly Type _asyncCmdHandlerType = typeof(IAsyncCommandHandler<>);
        private readonly Type _eventHandlerType = typeof(IEventHandler<>);
        private readonly Type _asyncEventHandlerType = typeof(IAsyncEventHandler<>);
        
        public IEnumerable<ICommandHandler<T>> GetCommandHandlers<T>() where T : Command
        {
            return ResolveHandlers<ICommandHandler<T>>(typeof(T), _cmdHandlerType);
        }

        public IEnumerable<IAsyncCommandHandler<T>> GetAsyncCommandHandlers<T>() where T : Command
        {
            return ResolveHandlers<IAsyncCommandHandler<T>>(typeof(T), _asyncCmdHandlerType);
        }

        private IEnumerable<THandler> ResolveHandlers<THandler>(Type messageType, Type genericHandlerType)
        {
            var key = new Tuple<Type, Type>(messageType, genericHandlerType);
            if (!_handlerTypeCache.ContainsKey(key))
            {
                _handlerTypeCache.TryAdd(key, genericHandlerType.MakeGenericType(messageType));
            }

            var type = _handlerTypeCache[key];
            return DependencyResolver.Current.GetServices(type).Select(h => (THandler)h);
        }

        public IEnumerable<IEventHandler<T>> GetEventHandlers<T>(Type eventType) where T : Event
        {
            var type = GetHandlerType(eventType, _eventHandlerType);
            return DependencyResolver.Current.GetServices(type).Select(h => (new EventHandlerAdapter<T>(h)));
        }

        public IEnumerable<IAsyncEventHandler<T>> GetAsyncEventHandlers<T>(Type eventType) where T : Event
        {
            var type = GetHandlerType(eventType, _asyncEventHandlerType);
            return DependencyResolver.Current.GetServices(type).Select(h => (new AsyncEventHandlerAdapter<T>(h)));
        }

        private Type GetHandlerType(Type eventType, Type genericHandlerType)
        {
            var key = new Tuple<Type, Type>(eventType, genericHandlerType);
            if (!_handlerTypeCache.ContainsKey(key))
            {
                _handlerTypeCache.TryAdd(key, genericHandlerType.MakeGenericType(eventType));
            }

            var type = _handlerTypeCache[key];
            return type;
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