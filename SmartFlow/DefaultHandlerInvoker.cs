using System;
using System.Linq;
using Smartflow.Core.CQRS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smartflow.Core
{
    /// <summary>
    /// Invoke a message together with executing filter attributes
    /// </summary>
    internal class DefaultHandlerInvoker : IHandlerInvoker
    {
        private readonly IEventPublisher _eventPublisher;

        public DefaultHandlerInvoker(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        protected virtual MessageHandledContext<T> InvokeHandleMethodWithFilters<T>(HandlerContext<T, ICommandHandler<T>> context, IList<IFilter> filters) where T : Command
        {
            Func<MessageHandledContext<T>> continuation = () =>
            {
                var events = context.Handler.Handle(context.Message);
                foreach (var e in events)
                {
                    _eventPublisher.Publish(e);
                }
                return new MessageHandledContext<T>(context, false /* canceled */, null /* exception */);
            };

            // need to reverse the filter list because the continuations are built up backward
            Func<MessageHandledContext<T>> thunk = filters.Reverse().Aggregate(continuation, (next, filter) => () => InvokeMessageFilter(filter, context, next));
            return thunk();
        }

        protected virtual MessageHandledContext<T> InvokeHandleMethodWithFilters<T>(HandlerContext<T, IEventHandler<T>> context, IList<IFilter> filters) where T : Event
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

        protected virtual Task<MessageHandledContext<T>> InvokeHandleMethodWithFiltersAsync<T>(HandlerContext<T, IAsyncCommandHandler<T>> context, IList<IFilter> filters) where T : Command
        {
            Func<Task<MessageHandledContext<T>>> continuation = async () =>
            {
                var events = await context.Handler.HandleAsync(context.Message).ConfigureAwait(false);
                foreach (var e in events)
                {
                    _eventPublisher.Publish(e);
                }
                return new MessageHandledContext<T>(context, false /* canceled */, null /* exception */);
            };

            // need to reverse the filter list because the continuations are built up backward
            Func<Task<MessageHandledContext<T>>> thunk = filters.Reverse().Aggregate(continuation, (next, filter) => () => InvokeMessageFilterAsync(filter, context, next));
            return thunk();
        }

        protected virtual Task<MessageHandledContext<T>> InvokeHandleMethodWithFiltersAsync<T>(HandlerContext<T, IAsyncEventHandler<T>> context, IList<IFilter> filters) where T : Event
        {
            Func<Task<MessageHandledContext<T>>> continuation = async () =>
            {
                await context.Handler.HandleAsync(context.Message).ConfigureAwait(false);
                return new MessageHandledContext<T>(context, false /* canceled */, null /* exception */);
            };

            // need to reverse the filter list because the continuations are built up backward
            Func<Task<MessageHandledContext<T>>> thunk = filters.Reverse().Aggregate(continuation, (next, filter) => () => InvokeMessageFilterAsync(filter, context, next));
            return thunk();
        }

        /// <summary>
        /// Invoke all filters and the handler method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="command"></param>
        public void InvokeHandler<T>(ICommandHandler<T> handler, T command) where T : Command
        {
            var context = new HandlerContext<T, ICommandHandler<T>>(command, handler);
            var filters = FilterProviders.Providers.GetFilters(context);
            var filterInfo = new FilterInfo<T>(filters);

            try
            {
                InvokeHandleMethodWithFilters(context, filterInfo.MessageFilters);
            }
            catch (Exception ex)
            {
                var exceptionContext = InvokeExceptionFilters<T>(context, filterInfo.ExceptionFilters, ex);
                if (!exceptionContext.ExceptionHandled)
                {
                    throw;
                }
            }
        }

        public Task InvokeHandlerAsync<T>(IAsyncCommandHandler<T> handler, T command) where T : Command
        {
            var context = new HandlerContext<T, IAsyncCommandHandler<T>>(command, handler);
            var filters = FilterProviders.Providers.GetFilters(context);
            var filterInfo = new FilterInfo<T>(filters);

            try
            {
                return InvokeHandleMethodWithFiltersAsync(context, filterInfo.MessageFilters);
            }
            catch (Exception ex)
            {
                var exceptionContext = InvokeExceptionFilters<T>(context, filterInfo.ExceptionFilters, ex);
                if (!exceptionContext.ExceptionHandled)
                {
                    throw;
                }
            }
            return Task.FromResult(false);
        }

        public void InvokeHandler<T>(IEventHandler<T> handler, T command) where T : Event
        {
            var context = new HandlerContext<T, IEventHandler<T>>(command, handler);
            var filters = FilterProviders.Providers.GetFilters(context);
            var filterInfo = new FilterInfo<T>(filters);

            try
            {
                InvokeHandleMethodWithFilters(context, filterInfo.MessageFilters);
            }
            catch (Exception ex)
            {
                var exceptionContext = InvokeExceptionFilters<T>(context, filterInfo.ExceptionFilters, ex);
                if (!exceptionContext.ExceptionHandled)
                {
                    throw;
                }
            }
        }

        public Task InvokeHandlerAsync<T>(IAsyncEventHandler<T> handler, T command) where T : Event
        {
            var context = new HandlerContext<T, IAsyncEventHandler<T>>(command, handler);
            var filters = FilterProviders.Providers.GetFilters(context);
            var filterInfo = new FilterInfo<T>(filters);

            try
            {
                return InvokeHandleMethodWithFiltersAsync(context, filterInfo.MessageFilters);
            }
            catch (Exception ex)
            {
                var exceptionContext = InvokeExceptionFilters<T>(context, filterInfo.ExceptionFilters, ex);
                if (!exceptionContext.ExceptionHandled)
                {
                    throw;
                }
            }
            return Task.FromResult(false);
        }

        

        internal static MessageHandledContext<T> InvokeMessageFilter<T>(IFilter filter, HandlerContext<T> preContext, Func<MessageHandledContext<T>> continuation)
            where T : class, IMessage
        {
            OnMessageExecuting(filter, preContext);
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

                OnMessageExecuted(filter, postContext);

                if (!postContext.ExceptionHandled)
                {
                    throw;
                }
            }

            if (!wasError)
            {
                OnMessageExecuted(filter, postContext);
            }
            return postContext;
        }

        internal static async Task<MessageHandledContext<T>> InvokeMessageFilterAsync<T>(IFilter filter, HandlerContext<T> preContext, Func<Task<MessageHandledContext<T>>> continuation)
            where T : class, IMessage
        {
            OnMessageExecuting(filter, preContext);
            if (preContext.MessageHandled)
            {
                return new MessageHandledContext<T>(preContext, true /* canceled */, null /* exception */);
            }

            bool wasError = false;
            MessageHandledContext<T> postContext;
            try
            {
                postContext = await continuation().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                wasError = true;
                postContext = new MessageHandledContext<T>(preContext, false /* canceled */, ex);

                OnMessageExecuted(filter, postContext);

                if (!postContext.ExceptionHandled)
                {
                    throw;
                }
            }

            if (!wasError)
            {
                OnMessageExecuted(filter, postContext);
            }
            return postContext;
        }

        private static void OnMessageExecuted<T>(IFilter filter, MessageHandledContext<T> postContext) where T : class, IMessage
        {
            if (filter is IMessageFilter<T>)
            {
                ((IMessageFilter<T>) filter).OnMessageExecuted(postContext);
            }
            else if (filter is IMessageFilter)
            {
                (filter as IMessageFilter).OnMessageExecuted(postContext);
            }
        }

        private static void OnMessageExecuting<T>(IFilter filter, HandlerContext<T> preContext) where T : class, IMessage
        {
            if (filter is IMessageFilter<T>)
            {
                ((IMessageFilter<T>) filter).OnMessageExecuting(preContext);
            }
            else if (filter is IMessageFilter)
            {
                (filter as IMessageFilter).OnMessageExecuting(preContext);
            }
        }

        protected virtual ExceptionContext InvokeExceptionFilters<T>(HandlerContext handlerContext, IList<IExceptionFilter> filters, Exception exception) where T : class, IMessage
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