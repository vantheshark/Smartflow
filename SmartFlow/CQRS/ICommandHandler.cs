using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// Handle a command and return a list of events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommandHandler<T> : IMessageHandler<T, HandlerContext<T, ICommandHandler<T>>> where T : Command
    {
        /// <summary>
        /// Handle a command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        IEnumerable<Event> Handle(T command);
    }

    /// <summary>
    /// Handle a command and return a list of events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAsyncCommandHandler<T> : IMessageHandler<T, HandlerContext<T, IAsyncCommandHandler<T>>> where T : Command
    {
        /// <summary>
        /// Handle a command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<IEnumerable<Event>> HandleAsync(T command);
    }
}