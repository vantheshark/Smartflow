using Burrow;

namespace Smartflow.RabbitMQ
{
    /// <summary>
    /// Default route finder for using with RabbitMQ and Burrow.NET
    /// </summary>
    internal class SmartflowRouteFinder : IRouteFinder
    {
        /// <summary>
        /// Default prefix name for exchanges and queues
        /// </summary>
        public static string ExchangeAndQueuePrefixName = "Smartflow";
        /// <summary>
        /// Environment string
        /// </summary>
        protected readonly string _environment;

        /// <summary>
        /// Initialize the route finder with the provided environment string
        /// </summary>
        /// <param name="environment"></param>
        public SmartflowRouteFinder(string environment)
        {
            _environment = environment;
        }

        /// <summary>
        /// Generate the exchange name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string FindExchangeName<T>()
        {
            return string.Format("{0}.E.HEADERS.{1}.{2}", ExchangeAndQueuePrefixName, _environment, typeof(T).Name);
        }

        /// <summary>
        /// Generate the routing key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string FindRoutingKey<T>()
        {
            return typeof (T).Name;
        }

        /// <summary>
        /// Generate the queue name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subscriptionName"></param>
        /// <returns></returns>
        public string FindQueueName<T>(string subscriptionName)
        {
            return string.Format("{0}.Q.{1}.{2}", ExchangeAndQueuePrefixName, _environment, typeof(T).Name);
        }
    }
}