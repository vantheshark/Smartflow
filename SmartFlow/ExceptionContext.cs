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
            HandlerType = handlerContext.HandlerType;
            MessageType = handlerContext.MessageType;
        }

        /// <summary>
        /// The exception
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Determine whether the exception was handled
        /// </summary>
        public bool ExceptionHandled { get; set; }
    }
}