using System;
using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    /// <summary>
    /// The context where message is successfully handled
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageHandledContext<T> : HandlerContext<T> where T : class, IMessage
    {
        /// <summary>
        /// Initialize the context by original HandlerContext
        /// </summary>
        /// <param name="handlerContext"></param>
        /// <param name="canceled"></param>
        /// <param name="exception"></param>
        public MessageHandledContext(HandlerContext<T> handlerContext, bool canceled, Exception exception)
            : base(handlerContext.Message, handlerContext.Handler)
        {
            Canceled = canceled;
            Exception = exception;
            MetaData = handlerContext.MetaData;
            MessageHandled = handlerContext.MessageHandled;
        }

        /// <summary>
        /// Check if the message handling was canceled
        /// </summary>
        public virtual bool Canceled { get; set; }

        /// <summary>
        /// Check if there was an exception
        /// </summary>
        public virtual Exception Exception { get; set; }

        /// <summary>
        /// Check if the exception if any was handled
        /// </summary>
        public bool ExceptionHandled { get; set; }
    }
}