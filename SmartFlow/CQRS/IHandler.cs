
using System;
using System.Linq;
using System.Reflection;

namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// Implement this interface to handle events and commands or anything implement  IMessage
    /// </summary>
    public interface IHandler<in T> where T : class, IMessage
    {
        /// <summary>
        /// Handle the message
        /// </summary>
        /// <param name="message"></param>
        void Handle(T message);
    }

    /// <summary>
    /// A base handler class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Handler<T> : IHandler<T> where T : class, IMessage
    {
        /// <summary>
        /// Initialize the CommandSender and EventPublisher
        /// </summary>
        protected Handler()
        {
            CommandSender = DependencyResolver.Current.GetService<ICommandSender>() ?? InternalBus.Current;
            EventPublisher = DependencyResolver.Current.GetService<IEventPublisher>() ?? InternalBus.Current;
        }

        /// <summary>
        /// The Command Sender which can be used to send new command during processing a message
        /// </summary>
        protected ICommandSender CommandSender { get; private set; }
        
        /// <summary>
        /// The Event Publisher which can be used to publish any events during processing a message
        /// </summary>
        protected IEventPublisher EventPublisher { get; private set; }

        /// <summary>
        /// Implement this method to handle the message
        /// </summary>
        /// <param name="message"></param>
        public abstract void Handle(T message);
        
        /// <summary>
        /// The context of the message when being handled
        /// </summary>
        public HandlerContext<T> Context { get; internal set; }
    }

    internal class HandlerWrapper<T> : Handler<IMessage> where T : class, IMessage
    {
        private readonly IHandler<T> _handler;

        public HandlerWrapper(IHandler<T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            _handler = handler;
        }

        public override void Handle(IMessage message)
        {
            _handler.Handle((T)message);
        }
    }

    internal class HandlerWrapper : Handler<IMessage>
    {
        private readonly object _handler;

        public HandlerWrapper(object handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            _handler = handler;
        }

        public override void Handle(IMessage message)
        {
            var type = typeof(HandlerContext<>);
            var handlerType = _handler.GetType();
            if (handlerType.Is(typeof(Handler<>)))
            {
                var @interface = handlerType.GetInterfaces().FirstOrDefault(i => i.Is(typeof(IHandler<>)) && i.GetGenericArguments()[0] == message.GetType());
                if (@interface != null)
                {
                    type = type.MakeGenericType(message.GetType());
                    var context = (HandlerContext) Activator.CreateInstance(type, message, _handler);
                    context.MessageHandled = Context.MessageHandled;
                    context.MetaData = Context.MetaData;
                }
            }

            handlerType.InvokeMember("Handle", BindingFlags.InvokeMethod, null, _handler, new object[] {message});
        }
    }
}