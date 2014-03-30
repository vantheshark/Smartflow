namespace Smartflow.RabbitMQ
{
    /// <summary>
    /// This is just a wrapper of IDistributedMessage to send to RabbitMQ queues
    /// </summary>
    public class SmartflowMessage
    {
        public object DistributedMessage { get; set; }
    }
}
