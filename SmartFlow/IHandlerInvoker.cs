using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    /// <summary>
    /// Implement this interface to invoke a message
    /// </summary>
    public interface IHandlerInvoker
    {
        /// <summary>
        /// Invoke a handler with the message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="message"></param>
        void InvokeHandler<T>(IHandler<T> handler, T message) where T : class, IMessage;
    }
}