using Burrow;
using Burrow.Extras;

namespace Smartflow.RabbitMQ
{
    public class SmartflowRouteFinder : IRouteFinder
    {
        public static string ExchangeAndQueuePrefixName = "Smartflow";
        private readonly string _environment;

        public SmartflowRouteFinder(string environment)
        {
            _environment = environment;
        }

        public string FindExchangeName<T>()
        {
            return string.Format("{0}.E.HEADERS.{1}.{2}", ExchangeAndQueuePrefixName, _environment, typeof(T).Name);
        }

        public string FindRoutingKey<T>()
        {
            return typeof (T).Name;
        }

        public string FindQueueName<T>(string subscriptionName)
        {
            return string.Format("{0}.Q.{1}.{2}", ExchangeAndQueuePrefixName, _environment, typeof(T).Name);
        }

        public void CreateRoutes<T>(string connectionString, uint maxPriority)
        {
            var setup = new PriorityQueuesRabbitSetup(connectionString);
            setup.CreateRoute<T>(new RouteSetupData
            {
                RouteFinder = this,
                ExchangeSetupData = new HeaderExchangeSetupData(),
                QueueSetupData = new PriorityQueueSetupData(maxPriority),
                
            });
        }
    }
}