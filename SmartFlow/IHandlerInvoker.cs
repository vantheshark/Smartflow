using System.Threading.Tasks;
using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    /// <summary>
    /// Implement this interface to invoke a message
    /// </summary>
    public interface IHandlerInvoker
    {
        /// <summary>
        /// Invoke a handler with the command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="command"></param>
        void InvokeHandler<T>(ICommandHandler<T> handler, T command) where T : Command;


        /// <summary>
        /// Invoke a handler with the command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="command"></param>
        void InvokeHandler<T>(IEventHandler<T> handler, T command) where T : Event;

        /// <summary>
        /// Invoke a handler asynchronously with the event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        Task InvokeHandlerAsync<T>(IAsyncCommandHandler<T> handler, T command) where T : Command;

        /// <summary>
        /// Invoke a handler asynchronously with the event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        Task InvokeHandlerAsync<T>(IAsyncEventHandler<T> handler, T command) where T : Event;
    }
}