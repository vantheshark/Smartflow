using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    internal class DefaultHandlerProvider : IHandlerProvider
    {
        private static readonly Dictionary<Type, object> CommandHandlers = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, object> AsyncCommandHandlers = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, List<object>> EventHandlers = new Dictionary<Type, List<object>>();
        private static readonly Dictionary<Type, List<object>> AsyncEventHandlers = new Dictionary<Type, List<object>>();

        
        public void RegisterCommandHandler<T>(ICommandHandler<T> handler) where T : Command
        {
            CommandHandlers[typeof (T)] = handler;
        }
        
        public void RegisterCommandHandler<T>(IAsyncCommandHandler<T> handler) where T : Command
        {
            AsyncCommandHandlers[typeof(T)] = handler;
        }

        public void RegisterEventHandler<T>(IEventHandler<T> handler) where T : Event
        {
            List<object> handlers;
            if (!EventHandlers.ContainsKey(typeof (T)))
            {
                handlers = new List<object>();
                EventHandlers[typeof (T)] = handlers;
            }

            handlers = EventHandlers[typeof (T)];
            handlers.Add(handler);
        }

        public void RegisterEventHandler<T>(IAsyncEventHandler<T> handler) where T : Event
        {
            List<object> handlers;
            if (!AsyncEventHandlers.ContainsKey(typeof(T)))
            {
                handlers = new List<object>();
                AsyncEventHandlers[typeof(T)] = handlers;
            }
            handlers = AsyncEventHandlers[typeof(T)];
            handlers.Add(handler);
        }

        public IEnumerable<ICommandHandler<T>> GetCommandHandlers<T>() where T : Command
        {
            if (CommandHandlers.ContainsKey(typeof (T)))
            {
                yield return (ICommandHandler<T>)CommandHandlers[typeof (T)];
            }
        }

        public IEnumerable<IAsyncCommandHandler<T>> GetAsyncCommandHandlers<T>() where T : Command
        {
            if (AsyncCommandHandlers.ContainsKey(typeof(T)))
            {
                yield return (IAsyncCommandHandler<T>)AsyncCommandHandlers[typeof(T)];
            }
        }

        public IEnumerable<IEventHandler<T>> GetEventHandlers<T>(Type eventType) where T : Event
        {
            if (EventHandlers.ContainsKey(eventType))
            {
                var handlers = EventHandlers[eventType];
                foreach (var h in handlers)
                {
                    yield return new EventHandlerAdapter<T>(h);
                }
            }
        }

        public IEnumerable<IAsyncEventHandler<T>> GetAsyncEventHandlers<T>(Type eventType) where T : Event
        {
            if (AsyncEventHandlers.ContainsKey(eventType))
            {
                var handlers = AsyncEventHandlers[eventType];
                foreach (var h in handlers)
                {
                    yield return new AsyncEventHandlerAdapter<T>(h);
                }
            }
        }
    }

    internal class EventHandlerAdapter<T> : CQRS.EventHandler<T> where T : Event
    {
        private readonly object _handler;

        public EventHandlerAdapter(object handler)
        {
            _handler = handler;
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
        }

        public override void Handle(T message)
        {
            _handler.GetType().InvokeMember("Handle", BindingFlags.InvokeMethod, null, _handler, new object[] { message });
        }
    }

    internal class AsyncEventHandlerAdapter<T> : AsyncEventHandler<T> where T : Event
    {
        private readonly object _handler;

        public AsyncEventHandlerAdapter(object handler)
        {
            _handler = handler;
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
        }

        public override Task HandleAsync(T message)
        {
            return (Task)_handler.GetType().InvokeMember("HandleAsync", BindingFlags.InvokeMethod, null, _handler, new object[] { message });
        }
    }
}