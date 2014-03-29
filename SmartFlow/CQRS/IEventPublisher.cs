namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// Implement this interface to publish events
    /// The default "InternalBus" implementation will be responsible for this as an internal memory bus
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publish an event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        void Publish<T>(T @event) where T : Event;
    }
}