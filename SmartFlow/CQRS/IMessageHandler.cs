namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// Implement this interface to handle a message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public interface IMessageHandler<T, TContext>
        where T : IMessage
        where TContext : HandlerContext<T>
    {
        /// <summary>
        /// The handler context
        /// </summary>
        TContext Context { get; }

        /// <summary>
        /// Initialize the handler with context
        /// </summary>
        /// <param name="context"></param>
        void Initialize(TContext context);
    }
}