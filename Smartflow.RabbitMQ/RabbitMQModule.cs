using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using Burrow;
using Smartflow.Core;
using Smartflow.Core.CQRS;
using Global = Smartflow.Core.Global;

namespace Smartflow.RabbitMQ
{
    /// <summary>
    /// Register this module to use RabbitMQ as the bus instead of <see cref="InternalBus"/>
    /// </summary>
    public class RabbitMqModule
    {
        /// <summary>
        /// Message queue connection string
        /// </summary>
        public string MessageQueueConnectionString { get; set; }
        /// <summary>
        /// The environment value, can be DEV, UAT, PROD. It will be a part of you RabbitMQ exchange and queue names
        /// </summary>
        public string Environment { get; set; }
        /// <summary>
        /// Set to true to let the library create the queue for you
        /// </summary>
        public bool SetupQueues { get; set; }

        private RabbitMqBus _rabbitBus;

        /// <summary>
        /// Init the module with provided <see cref="LogicalPriorityMapper"/>
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="routeFinder">custom route finder</param>
        /// <returns></returns>
        public RabbitMqModule Load(LogicalPriorityMapper mapper, IRouteFinder routeFinder = null)
        {
            var internalBus = InternalBus.Current;
            var logger = DependencyResolver.Current.GetService<ILogger>() ?? new NullLogger();
            _rabbitBus = new RabbitMqBus(logger, internalBus, mapper, MessageQueueConnectionString, Environment, SetupQueues);
            if (routeFinder != null)
            {
                _rabbitBus.SetRouteFinder(routeFinder);
            }
            InternalBus.Current = new RabbitMqBusAdapter(_rabbitBus);

            Global.PendingJobCount = _rabbitBus.GetMessageCount;
            Burrow.Global.DefaultConsumerBatchSize = ushort.Parse(ConfigurationManager.AppSettings["TaskConcurrencyLevel"] ?? "4");
            Burrow.Global.PreFetchSize = uint.Parse(ConfigurationManager.AppSettings["PreFetchSize"] ?? ((Burrow.Global.DefaultConsumerBatchSize) * 2).ToString(CultureInfo.InvariantCulture));
            return this;
        }

        /// <summary>
        /// Start subscribing to the priority queues
        /// </summary>
        public void Start()
        {
            _rabbitBus.Start();
        }

        /// <summary>
        /// Init the module from App.Config
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public RabbitMqModule WithConfig(NameValueCollection collection)
        {
            MessageQueueConnectionString = collection["MessageQueueConnectionString"];
            Environment = collection["Environment"] ?? "DEV";
            SetupQueues = "true".Equals(collection["SetupQueues"], StringComparison.OrdinalIgnoreCase);
            return this;
        }
    }
}
