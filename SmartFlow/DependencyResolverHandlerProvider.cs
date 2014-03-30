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
        private readonly Type _genericType = typeof(IHandler<>);
        

        public IEnumerable<IHandler<IMessage>> GetHandlers(Type messageType)
        {
            if (!_handlerTypeCache.ContainsKey(messageType))
            {
                _handlerTypeCache.TryAdd(messageType, _genericType.MakeGenericType(messageType));
            }
            
            var handlerType = _handlerTypeCache[messageType];
            var handlers = DependencyResolver.Current.GetServices(handlerType).Select(h => new HandlerWrapper(h));
            foreach(var h in handlers)
            {
                yield return h;
            }
            
        }
    }
}