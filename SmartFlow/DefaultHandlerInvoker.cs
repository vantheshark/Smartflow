using System;
using System.Linq;
using Smartflow.Core.CQRS;
using System.Collections.Generic;

namespace Smartflow.Core
{
    /// <summary>
    /// Invoke a message together with executing filter attributes
    /// </summary>
    internal class DefaultHandlerInvoker : IHandlerInvoker
    {
        /// <summary>
        /// Invoke all filters and the handler method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="message"></param>
        public void InvokeHandler<T>(IHandler<T> handler, T message) where T : class, IMessage
        {
            var context = new HandlerContext<T>(message, handler);
            var filters = FilterProviders.Providers.GetFilters(context);
            var filterInfo = new FilterInfo<T>(filters);

            try
            {
                InvokeHandleMethodWithFilters(context, filterInfo.MessageFilters);
            }
            catch (Exception ex)
            {
                var exceptionContext = InvokeExceptionFilters(context, filterInfo.ExceptionFilters, ex);
                if (!exceptionContext.ExceptionHandled)
                {
                    throw;
                }
            }
        }

        protected virtual MessageHandledContext<T> InvokeHandleMethodWithFilters<T>(HandlerContext<T> context, IList<IFilter> filters) where T : class, IMessage
        {
            Func<MessageHandledContext<T>> continuation = () =>
            {
                context.Handler.Handle(context.Message);
                return new MessageHandledContext<T>(context, false /* canceled */, null /* exception */);
            };

            // need to reverse the filter list because the continuations are built up backward
            Func<MessageHandledContext<T>> thunk = filters.Reverse().Aggregate(continuation, (next, filter) => () => InvokeMessageFilter(filter, context, next));
            return thunk();
        }

        internal static MessageHandledContext<T> InvokeMessageFilter<T>(IFilter filter, HandlerContext<T> preContext, Func<MessageHandledContext<T>> continuation) where T : class, IMessage
        {
            if (filter is IMessageFilter<T>)
            {
                ((IMessageFilter<T>)filter).OnMessageExecuting(preContext);
            }
            else if (filter is IMessageFilter)
            {
                (filter as IMessageFilter).OnMessageExecuting(preContext);
            }
            if (preContext.MessageHandled)
            {
                return new MessageHandledContext<T>(preContext, true /* canceled */, null /* exception */);
            }

            bool wasError = false;
            MessageHandledContext<T> postContext;
            try
            {
                postContext = continuation();
            }
            catch (Exception ex)
            {
                wasError = true;
                postContext = new MessageHandledContext<T>(preContext, false /* canceled */, ex);

                if (filter is IMessageFilter<T>)
                {
                    ((IMessageFilter<T>)filter).OnMessageExecuted(postContext);
                }
                else if (filter is IMessageFilter)
                {
                    (filter as IMessageFilter).OnMessageExecuted(postContext);
                }


                if (!postContext.ExceptionHandled)
                {
                    throw;
                }
            }

            if (!wasError)
            {
                if (filter is IMessageFilter<T>)
                {
                    ((IMessageFilter<T>)filter).OnMessageExecuted(postContext);
                }
                else if (filter is IMessageFilter)
                {
                    (filter as IMessageFilter).OnMessageExecuted(postContext);
                }
            }
            return postContext;
        }

        protected virtual ExceptionContext InvokeExceptionFilters<T>(HandlerContext<T> handlerContext, IList<IExceptionFilter> filters, Exception exception) where T : class, IMessage
        {
            var context = new ExceptionContext(handlerContext, exception);
            foreach (var filter in filters.Reverse())
            {
                filter.OnException(context);
            }

            return context;
        }
    }
}