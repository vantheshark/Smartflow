using System;
using System.Collections.Concurrent;
using System.Threading;
using Burrow;
using Burrow.Extras;
using Burrow.Extras.Internal;
using Smartflow.Core;
using Smartflow.Core.CQRS;
using Global = Burrow.Global;

namespace Smartflow.RabbitMQ
{
    public class RabbitMqBus : ICommandSender, IEventPublisher, IDisposable
    {
        private readonly ILogger _logger;
        private readonly InternalBus _internalBus;
        private readonly LogicalPriorityMapper _priorityMapper;
        private readonly SmartflowRouteFinder _routeFinder;

        private readonly string _connectionString;
        private readonly uint _maxPriority;
        private readonly bool _setupQueues;
        private readonly ITunnelWithPrioritySupport _tunnel;
        private volatile CompositeSubscription _subscription;
        private readonly ConcurrentDictionary<IMessage, MessageDeliverEventArgs> _cache = new ConcurrentDictionary<IMessage, MessageDeliverEventArgs>();
        private Semaphore _sem;
        private readonly ManualResetEvent _waitToStart = new ManualResetEvent(false);

        public RabbitMqBus(ILogger logger, InternalBus internalBus, LogicalPriorityMapper priorityMapper, string connectionString, string environment, bool setupQueues = false)
        {
            _logger = logger;
            _internalBus = internalBus;
            _priorityMapper = priorityMapper;
            _connectionString = connectionString;
            _maxPriority = _priorityMapper.GetQueuePriority(int.MaxValue);
            _setupQueues = setupQueues;
            _routeFinder = new SmartflowRouteFinder(environment);
            _tunnel = RabbitTunnel.Factory.WithPrioritySupport()
                                  .Create(_connectionString).WithPrioritySupport();
            _tunnel.SetRouteFinder(_routeFinder);
            _tunnel.SetSerializer(new JsonSerializer());
        }

        public uint GetMessageCount()
        {
            return _tunnel.GetMessageCount(new PrioritySubscriptionOption<SmartflowMessage>
            {
                MaxPriorityLevel = _maxPriority
            });
        }

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

        public virtual void Start()
        {
            if (_setupQueues)
            {
                _routeFinder.CreateRoutes<SmartflowMessage>(_connectionString, _maxPriority);
            }

            _sem = new Semaphore(Global.DefaultConsumerBatchSize, Global.DefaultConsumerBatchSize);

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
