using Smartflow.RabbitMQ;

namespace Smartflow.Demo
{
    /// <summary>
    /// Any messages with priority >=5 will be send to RabbitMQ queue _Priority5
    /// </summary>
    public class LogicalPriorityMapperDemo : LogicalPriorityMapper
    {
        public override uint GetQueuePriority(uint logicalPriority)
        {
            return logicalPriority < 5 ? logicalPriority : 5;
        }
    }
}