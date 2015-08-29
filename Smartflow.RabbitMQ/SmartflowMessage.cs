namespace Smartflow.RabbitMQ
{
    /// <summary>
    /// This is just a wrapper of IDistributedMessage to send to RabbitMQ queues
    /// </summary>
    public class SmartflowMessage
    {
        /// <summary>
        /// An embeded object for the distributed Command or Event
        /// </summary>
        public object DistributedMessage { get; set; }
    }
}
