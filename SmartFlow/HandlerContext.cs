using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    /// <summary>
    /// The handler context
    /// </summary>
    public abstract class HandlerContext
    {
        /// <summary>
        /// The type of the handler
        /// </summary>
        public Type HandlerType { get; protected set; }
        /// <summary>
        /// The type of the message
        /// </summary>
        public Type MessageType { get; protected set; }

        /// <summary>
        /// Determine whether the message was handled
        /// </summary>
        public bool MessageHandled { get; protected set; }

        /// <summary>
        /// The share dictionary of objects that can be set/get during handling the message
        /// </summary>
        public IDictionary<object, object> MetaData { get; internal set; }
    }

    /// <summary>
    /// The handler context
    /// </summary>
    public abstract class HandlerContext<T> : HandlerContext
    {
        /// <summary>
        /// The message being handled
        /// </summary>
        public T Message { get; protected set; }
    }

    /// <summary>
    /// The handler context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="THandler"></typeparam>
    public class HandlerContext<T, THandler> : HandlerContext<T> 
        where T: IMessage
        where THandler : IMessageHandler<T, HandlerContext<T, THandler>>
    {
        
        /// <summary>
        /// The handler instance
        /// </summary>
        public THandler Handler { get; internal set; }
        
        /// <summary>
        /// Initialize a handler context by message and its handler
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="handler"></param>
        public HandlerContext(T msg, THandler handler)
        {
            Message = msg;
            Handler = handler;
            handler.Initialize(this);
            MetaData = new ConcurrentDictionary<object, object>();
            HandlerType = handler.GetType();
            MessageType = msg.GetType();
        }
    }
}