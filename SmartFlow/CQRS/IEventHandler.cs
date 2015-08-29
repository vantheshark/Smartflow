using System.Threading.Tasks;

namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// Implement this interface to handle the event T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventHandler<T> : IMessageHandler<T, HandlerContext<T, IEventHandler<T>>> where T : Event
    {
        /// <summary>
        /// Handle the event
        /// </summary>
        /// <param name="event"></param>
        void Handle(T @event);
    }

    /// <summary>
    /// Implement this interface to handle the event T asynchronously
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAsyncEventHandler<T> : IMessageHandler<T, HandlerContext<T, IAsyncEventHandler<T>>> where T : Event
    {
        /// <summary>
        /// Handle the event
        /// </summary>
        /// <param name="event"></param>
        Task HandleAsync(T @event);
    }
}