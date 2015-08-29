using System;
using System.Linq;
using System.Threading.Tasks;
using Smartflow.Core.Tasks;

namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// This implementation is inspired bu Craig Young's implementation in his CQRS example.
    /// There should be 1 singleton object of this class in the system to act as the internal bus which can send/receive messages and deliver them
    /// to the <see cref="LimitedConcurrencyLevelTaskScheduler"/>
    /// </summary>
    public class InternalBus : ICommandSender, IEventPublisher
    {
        private class Nested
        {
            internal static InternalBus _instance = new InternalBus(null, null);
            static Nested()
            {
            }
        }

        /// <summary>
        /// The current internal bus
        /// </summary>
        public static InternalBus Current
        {
            get { return Nested._instance; }
            set { Nested._instance = value; }
        }


        private readonly ILogger _logger;
        private readonly IHandlerInvoker _handlerInvoker;

        /// <summary>
        /// A callback method to handle once the message is handled
        /// </summary>
        public Action<IMessage> MesageHandled { get; set; }

        /// <summary>
        /// Initialize an internal bus. This object should be singleton
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="handlerInvoker"> </param>
        public InternalBus(ILogger logger, IHandlerInvoker handlerInvoker)
        {
            _logger = logger ?? DependencyResolver.Current.GetService<ILogger>() ?? new NullLogger();
            _handlerInvoker = handlerInvoker ?? DependencyResolver.Current.GetService<IHandlerInvoker>() ?? new DefaultHandlerInvoker(this);
        }
        
        /// <summary>
        /// Send a command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        public virtual void Send<T>(T command) where T : Command
        {
            var handlers = HandlerProvider.Providers.GetCommandHandlers<T>().ToList();
            Action a = null;
            if (handlers.Count > 0)
            {
                if (handlers.Count != 1)
                {
                    throw new InvalidOperationException(string.Format("Cannot send {0} to more than one handler",
                        typeof (T).Name));
                }
                var syncHandler = handlers[0];
                a = () => { _handlerInvoker.InvokeHandler(syncHandler, command); };
            }

            if (a == null)
            {
                var asyncHandlers = HandlerProvider.Providers.GetAsyncCommandHandlers<T>().ToList();
                if (asyncHandlers.Count > 0)
                {
                    if (asyncHandlers.Count != 1)
                    {
                        throw new InvalidOperationException(string.Format("Cannot send {0} to more than one handler", typeof(T).Name));
                    }
                    var asyncHandler = asyncHandlers[0];
                    a = async () => await _handlerInvoker.InvokeHandlerAsync(asyncHandler, command).ConfigureAwait(false);
                }
            }

            if (a == null)
            {
                throw new InvalidOperationException("no handler registered for " + typeof(T).Name);
            }
            
            var priorityTask = CreatePriorityTask(command, a);

            priorityTask.OnDemand = command.Priority == (uint)MessagePriority.Highest;
            priorityTask.Priority = command.Priority;
            priorityTask.Start(PriorityTask.Factory.Scheduler);
        }

        /// <summary>
        /// Execute a query for result immediately
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<T> Query<T>(Query<T> query)
        {
            var handlers = HandlerProvider.Providers.GetCommandHandlers<Query<T>>().ToList();
            if (handlers.Count > 0)
            {
                if (handlers.Count != 1)
                    throw new InvalidOperationException("cannot send to more than one handler");
                var handler1 = handlers[0];

                return Task<T>.Factory.StartNew(() =>
                {
                    _handlerInvoker.InvokeHandler(handler1, query);
                    return query.Result;
                }, TaskCreationOptions.PreferFairness);
            }


            var asyncHandlers = HandlerProvider.Providers.GetAsyncCommandHandlers<Query<T>>().ToList();
            if (asyncHandlers.Count > 0)
            {
                if (asyncHandlers.Count != 1)
                {
                    throw new InvalidOperationException(string.Format("Cannot send {0} to more than one handler", typeof(T).Name));
                }
                var asyncHandler = asyncHandlers[0];
                return Task.Run(async () =>
                {
                    await _handlerInvoker.InvokeHandlerAsync(asyncHandler, query).ConfigureAwait(false);
                    return query.Result;
                });
            }
            
            throw new InvalidOperationException("no handler registered");
        }

        /// <summary>
        /// Publish an event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        public virtual void Publish<T>(T @event) where T : Event
        {
            var handlers = HandlerProvider.Providers.GetEventHandlers<T>(@event.GetType());
            foreach (var handler in handlers)
            {
                var h = handler;
                var priorityTask = CreatePriorityTask(@event, () => { h.Handle(@event); });

                priorityTask.OnDemand = @event.Priority == (uint)MessagePriority.Highest;
                priorityTask.Priority = @event.Priority;
                priorityTask.Start(PriorityTask.Factory.Scheduler);
            }

            var asyncHandlers = HandlerProvider.Providers.GetAsyncEventHandlers<T>(@event.GetType());
            foreach (var handler in asyncHandlers)
            {
                var h = handler;
                var priorityTask = CreatePriorityTask(@event, async () => { await h.HandleAsync(@event).ConfigureAwait(false); });
                priorityTask.OnDemand = @event.Priority == (uint)MessagePriority.Highest;
                priorityTask.Priority = @event.Priority;
                priorityTask.Start(PriorityTask.Factory.Scheduler);
            }
        }

        private PriorityTask CreatePriorityTask(IMessage message, Action a)
        {
            var priorityTask = new PriorityTask(() =>
            {
                try
                {
                    a();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                finally
                {
                    if (MesageHandled != null)
                    {
                        try
                        {
                            //NOTE: This can be fired many times for 1 event object as the event can have many handlers
                            MesageHandled(message);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                    }
                }
            });
            return priorityTask;
        }
    }
}
