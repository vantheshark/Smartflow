using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// A base handler class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CommandHandler<T> : ICommandHandler<T> where T : Command
    {
        /// <summary>
        /// Initialize the CommandSender and EventPublisher
        /// </summary>
        protected CommandHandler()
        {
            CommandSender = DependencyResolver.Current.GetService<ICommandSender>() ?? InternalBus.Current;
        }

        /// <summary>
        /// The Command Sender which can be used to send new command during processing a message
        /// </summary>
        protected ICommandSender CommandSender { get; private set; }

        /// <summary>
        /// Implement this method to handle the message
        /// </summary>
        /// <param name="message"></param>
        public abstract IEnumerable<Event> Handle(T message);
        
        /// <summary>
        /// The context of the message when being handled
        /// </summary>
        public HandlerContext<T, ICommandHandler<T>> Context { get; private set; }

        public void Initialize(HandlerContext<T, ICommandHandler<T>> context)
        {
            Context = context;
        }
    }

    /// <summary>
    /// A base handler class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AsyncCommandHandler<T> : IAsyncCommandHandler<T> where T : Command
    {
        /// <summary>
        /// Initialize the CommandSender and EventPublisher
        /// </summary>
        protected AsyncCommandHandler()
        {
            CommandSender = DependencyResolver.Current.GetService<ICommandSender>() ?? InternalBus.Current;
        }

        /// <summary>
        /// The Command Sender which can be used to send new command during processing a message
        /// </summary>
        protected ICommandSender CommandSender { get; private set; }

        /// <summary>
        /// Implement this method to handle the message
        /// </summary>
        /// <param name="message"></param>
        public abstract Task<IEnumerable<Event>> HandleAsync(T message);

        /// <summary>
        /// The context of the message when being handled
        /// </summary>
        public HandlerContext<T, IAsyncCommandHandler<T>> Context { get; private set; }

        public void Initialize(HandlerContext<T, IAsyncCommandHandler<T>> context)
        {
            Context = context;
        }
    }

    /// <summary>
    /// A base handler class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EventHandler<T> : IEventHandler<T> where T : Event
    {
        /// <summary>
        /// Implement this method to handle the message
        /// </summary>
        /// <param name="message"></param>
        public abstract void Handle(T message);

        /// <summary>
        /// The context of the message when being handled
        /// </summary>
        public HandlerContext<T, IEventHandler<T>> Context { get; private set; }

        public void Initialize(HandlerContext<T, IEventHandler<T>> context)
        {
            Context = context;
        }
    }

    /// <summary>
    /// A base handler class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AsyncEventHandler<T> : IAsyncEventHandler<T> where T : Event
    {
        /// <summary>
        /// Implement this method to handle the message
        /// </summary>
        /// <param name="event"></param>
        public abstract Task HandleAsync(T @event);

        /// <summary>
        /// The context of the message when being handled
        /// </summary>
        public HandlerContext<T, IAsyncEventHandler<T>> Context { get; private set; }

        public void Initialize(HandlerContext<T, IAsyncEventHandler<T>> context)
        {
            Context = context;
        }
    }

    
}