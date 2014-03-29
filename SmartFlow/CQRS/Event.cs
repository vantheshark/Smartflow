
namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// An event publisher should publish an Event and doesn't care which handler will handle it
    /// In other words, an event can be handled by more than 1 handler
    /// </summary>
    public abstract class Event : IMessage
    {
        /// <summary>
        /// An event should have a priority to be processed. The higher priority it is, the faster the event will be processed
        /// </summary>
        public uint Priority { get; set; }
    }
}
