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
        
        public abstract Type HandlerType { get; }
        /// <summary>
        /// The type of the message
        /// </summary>
        public abstract Type MessageType { get; }

        /// <summary>
        /// Determine whether the message was handled
        /// </summary>
        public bool MessageHandled { get; set; }

        /// <summary>
        /// The share dictionary of objects that can be set/get during handling the message
        /// </summary>
        public IDictionary<object, object> MetaData { get; internal set; }
    }

    /// <summary>
    /// The handler context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HandlerContext<T> : HandlerContext where T: class, IMessage
    {
        /// <summary>
        /// The message being handled
        /// </summary>
        public T Message { get; private set; }
        
        /// <summary>
        /// The handler instance
        /// </summary>
        public IHandler<T> Handler { get; internal set; }
        
        /// <summary>
        /// Initialize a handler context by message and its handler
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="handler"></param>
        public HandlerContext(T msg, IHandler<T> handler)
        {
            Message = msg;
            Handler = handler;
            if (handler is Handler<T>)
            {
                (handler as Handler<T>).Context = this;
            }
            MetaData = new ConcurrentDictionary<object, object>();
        }

        /// <summary>
        /// The type of the handler
        /// </summary>
        public override Type HandlerType
        {
            get { return Handler != null ? Handler.GetType() : null; }
        }

        /// <summary>
        /// The type of the message
        /// </summary>
        public override Type MessageType
        {
            get { return Message != null ? Message.GetType() : null; }
        }
    }
}