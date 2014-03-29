using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    /// <summary>
    /// Implement this interface to provide a filter logic for a message handling process without knowing the message and handler instances
    /// <para>Normally, global filters should implement this</para>
    /// </summary>
    public interface IMessageFilter : IFilter
    {
        /// <summary>
        /// This method is called before message is executed
        /// </summary>
        /// <param name="context"></param>
        void OnMessageExecuting(HandlerContext context);

        /// <summary>
        /// This method is called after message is executed
        /// </summary>
        /// <param name="context"></param>
        void OnMessageExecuted(HandlerContext context);
    }

    /// <summary>
    /// Implement this interface to provide a filter logic for a message handling process
    /// <para>Normally, attribute filter should implement this</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMessageFilter<T> : IFilter where T : class, IMessage
    {
        /// <summary>
        /// This method is called before message is executed
        /// </summary>
        /// <param name="context"></param>
        void OnMessageExecuting(HandlerContext<T> context);
        
        /// <summary>
        /// This method is called after message is executed
        /// </summary>
        /// <param name="context"></param>
        void OnMessageExecuted(MessageHandledContext<T> context);
    }
}