namespace Smartflow.RabbitMQ
{
    /// <summary>
    /// Implement this interface to provide a mapping logic of converting any logical uint priority using in the application to phisical rabbitMQ priority queue
    /// </summary>
    public abstract class LogicalPriorityMapper
    {
        /// <summary>
        /// Get the priority value of the queue for the provided message priority
        /// </summary>
        /// <param name="logicalPriority"></param>
        /// <returns></returns>
        public abstract uint GetQueuePriority(uint logicalPriority);
    }
}