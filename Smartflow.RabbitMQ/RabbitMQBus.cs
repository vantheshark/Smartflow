using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Burrow;
using Burrow.Extras;
using Burrow.Extras.Internal;
using Smartflow.Core;
using Smartflow.Core.CQRS;

namespace Smartflow.RabbitMQ
{
    /// <summary>
    /// The bus implementation using RabbitMQ and Burrow.NET
    /// </summary>
    public class RabbitMqBus : ICommandSender, IEventPublisher, IDisposable
    {
        private readonly ILogger _logger;
        private readonly InternalBus _internalBus;
        private readonly LogicalPriorityMapper _priorityMapper;
        private IRouteFinder _routeFinder;

        private readonly string _connectionString;
        private readonly uint _maxPriority;
        private readonly bool _setupQueues;
        private ITunnelWithPrioritySupport _tunnel;
        private volatile CompositeSubscription _subscription;
        private readonly ConcurrentDictionary<IMessage, MessageDeliverEventArgs> _cache = new ConcurrentDictionary<IMessage, MessageDeliverEventArgs>();
        private Semaphore _sem;
        private readonly ManualResetEvent _waitToStart = new ManualResetEvent(false);

        /// <summary>
        /// Initialize the bus
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="internalBus"></param>
        /// <param name="priorityMapper"></param>
        /// <param name="connectionString"></param>
        /// <param name="environment"></param>
        /// <param name="setupQueues"></param>
        public RabbitMqBus(ILogger logger, InternalBus internalBus, LogicalPriorityMapper priorityMapper, string connectionString, string environment, bool setupQueues = false)
        {
            _logger = logger;
            _internalBus = internalBus;
            _priorityMapper = priorityMapper;
            _connectionString = connectionString;
            _maxPriority = _priorityMapper.GetQueuePriority(int.MaxValue);
            _setupQueues = setupQueues;
            _routeFinder = new SmartflowRouteFinder(environment);
        }

        /// <summary>
        /// Change the route finder
        /// </summary>
        /// <param name="routeFinder"></param>
        public void SetRouteFinder(IRouteFinder routeFinder)
        {
            _routeFinder = routeFinder;
        }

        /// <summary>
        /// Count all messages from all the queues
        /// </summary>
        /// <returns></returns>
        public uint GetMessageCount()
        {
            return _tunnel.GetMessageCount(new PrioritySubscriptionOption<SmartflowMessage>
            {
                MaxPriorityLevel = _maxPriority
            });
        }

        /// <summary>
        /// Send the distributed command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        public void Send<T>(T command) where T : Command
        {
            if (command is IDistributedMessage)
            {
                _tunnel.Publish(new SmartflowMessage
                                    {
                                        DistributedMessage = command
                                    }, _priorityMapper.GetQueuePriority(command.Priority));
            }
            else
            {
                _internalBus.Send(command);
            }
        }

        /// <summary>
        /// Execute a query for result immediately
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<T> Query<T>(Query<T> query)
        {
            return _internalBus.Query(query);
        }

        /// <summary>
        /// Publish the distributed event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        public void Publish<T>(T @event) where T : Event
        {
            if (@event is IDistributedMessage)
            {
                _tunnel.Publish(new SmartflowMessage
                                    {
                                        DistributedMessage = @event
                                    }, _priorityMapper.GetQueuePriority(@event.Priority));
            }
            else
            {
                _internalBus.Publish(@event);
            }
        }

        /// <summary>
        /// Start the bus
        /// </summary>
        public virtual void Start()
        {
            _tunnel = RabbitTunnel.Factory.WithPrioritySupport()
                                  .Create(_connectionString).WithPrioritySupport();
            _tunnel.SetRouteFinder(_routeFinder);
            _tunnel.SetSerializer(new JsonSerializer());

            if (_setupQueues)
            {
                CreateRoutes<SmartflowMessage>(_connectionString, _maxPriority);
            }


            _sem = new Semaphore(Burrow.Global.DefaultConsumerBatchSize, Burrow.Global.DefaultConsumerBatchSize);

            CreateSubscription();

            ApplyCallBack();

            _waitToStart.Set();
        }

        private void ApplyCallBack()
        {
            _internalBus.MesageHandled += msg =>
            {
                try
                {
                    MessageDeliverEventArgs evt;
                    if (_cache.TryRemove(msg, out evt))
                    {
                        _subscription.Ack(evt.ConsumerTag, evt.DeliveryTag);
                        _sem.Release();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            };
        }

        /// <summary>
        /// Create Burrow.NET subscription to the queues
        /// </summary>
        protected virtual void CreateSubscription()
        {
            var name = string.Format("BuzzPollMonitor.{0}", Environment.MachineName);
            _subscription = _tunnel.SubscribeAsync(new PriorityAsyncSubscriptionOption<SmartflowMessage>
            {
                MaxPriorityLevel = _maxPriority,
                SubscriptionName = name,
                MessageHandler = (msg, evt) =>
                {
                    _waitToStart.WaitOne();
                    _sem.WaitOne();
                    if (msg.DistributedMessage == null)
                    {
                        _logger.FatalFormat("Received a null distributed message");
                        _subscription.Ack(evt.ConsumerTag, evt.DeliveryTag);
                    }
                    else if (msg.DistributedMessage is Command)
                    {
                        _cache[msg.DistributedMessage as IMessage] = evt;
                        _internalBus.Send(msg.DistributedMessage as Command);
                    }
                    else if (msg.DistributedMessage is Event)
                    {
                        _cache[msg.DistributedMessage as IMessage] = evt;
                        _internalBus.Publish(msg.DistributedMessage as Event);
                    }
                    else
                    {
                        _logger.FatalFormat("Message of type {0} is not supported", msg.GetType().Name);
                        _subscription.Ack(evt.ConsumerTag, evt.DeliveryTag);
                    }
                }
            });
        }

        /// <summary>
        /// Create the required exchange and queues
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="maxPriority"></param>
        private void CreateRoutes<T>(string connectionString, uint maxPriority)
        {
            var setup = new PriorityQueuesRabbitSetup(connectionString);
            setup.CreateRoute<T>(new RouteSetupData
            {
                RouteFinder = _routeFinder,
                ExchangeSetupData = new HeaderExchangeSetupData(),
                QueueSetupData = new PriorityQueueSetupData(maxPriority)
            });
        }

        /// <summary>
        /// Dispose the RabbitMQ bus
        /// </summary>
        public virtual void Dispose()
        {
            if (_sem != null)
            {
                _sem.Dispose();
            }
            _subscription.CancelAll();
            _tunnel.Dispose();
            _waitToStart.Dispose();
        }
    }
}
