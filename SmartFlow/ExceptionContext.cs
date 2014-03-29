using System;

namespace Smartflow.Core
{
    /// <summary>
    /// The Exception context
    /// </summary>
    public class ExceptionContext : HandlerContext
    {
        /// <summary>
        /// Initialize an instance of ExceptionContext
        /// </summary>
        /// <param name="handlerContext"></param>
        /// <param name="exception"></param>
        public ExceptionContext(HandlerContext handlerContext, Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            Exception = exception;
            MetaData = handlerContext.MetaData;
            MessageHandled = handlerContext.MessageHandled;
            _handlerType = handlerContext.HandlerType;
            _messageType = handlerContext.MessageType;
        }

        /// <summary>
        /// The exception
        /// </summary>
        public virtual Exception Exception { get; set; }

        /// <summary>
        /// Determine whether the exception was handled
        /// </summary>
        public bool ExceptionHandled { get; set; }

        private readonly Type _handlerType;
        private readonly Type _messageType;

        /// <summary>
        /// The type of the handler
        /// </summary>
        public override Type HandlerType { get { return _handlerType; }}

        /// <summary>
        /// The type of the message
        /// </summary>
        public override Type MessageType { get { return _messageType; }}
    }
}